using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        private async Task<Organiser?> GetCurrentOrganiserAsync(
            int userId)
        {
            return await _context.Organisers
                .FirstOrDefaultAsync(o => o.user_id == userId);
        }

        private async Task<Participant?> GetCurrentParticipantAsync(
            int userId)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(p => p.user_id == userId);
        }

        /// <summary>
        /// Records a finish result for an enrolment.
        /// Organisers can only record results for their own events.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromBody] ResultRequest request)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var organiser = await GetCurrentOrganiserAsync(userId);

            if (organiser == null)
            {
                return Forbid();
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .Include(e => e.Participant)
                    .ThenInclude(p => p!.User)
                .FirstOrDefaultAsync(
                    e => e.enrolment_id == request.EnrolmentId);

            if (enrolment == null ||
                enrolment.Category?.Event == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            if (enrolment.Category.Event.organiser_id !=
                organiser.organiser_id)
            {
                return Forbid();
            }

            bool resultExists = await _context.Results
                .AnyAsync(r =>
                    r.enrolment_id == request.EnrolmentId);

            if (resultExists)
            {
                return Conflict(new
                {
                    message = "A result already exists for this enrolment."
                });
            }

            if (request.FinishTime <= TimeSpan.Zero)
            {
                return BadRequest(new
                {
                    message = "Finish time must be greater than zero."
                });
            }

            var result = new Result
            {
                enrolment_id = request.EnrolmentId,
                finish_time = request.FinishTime,
                finishing_position = request.FinishingPosition
            };

            _context.Results.Add(result);

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Result recorded successfully.",
                result.result_id,
                result.enrolment_id,
                result.finish_time,
                result.finishing_position
            });
        }

        /// <summary>
        /// Allows a participant to view their own results.
        /// </summary>
        [Authorize(Roles = "Participant")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var participant = await GetCurrentParticipantAsync(userId);

            if (participant == null)
            {
                return NotFound(new
                {
                    message = "Participant profile not found."
                });
            }

            var results = await _context.Results
                .AsNoTracking()
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                        .ThenInclude(c => c!.Event)
                .Where(r =>
                    r.Enrolment!.participant_id ==
                    participant.participant_id)
                .OrderBy(r => r.finishing_position)
                .ToListAsync();

            var response = results.Select(r => new
            {
                r.result_id,
                r.finish_time,
                r.finishing_position,
                enrolmentId = r.enrolment_id,
                categoryName = r.Enrolment!.Category!.category_name,
                eventId = r.Enrolment.Category.Event!.event_id,
                eventName = r.Enrolment.Category.Event.event_name,
                eventDate = r.Enrolment.Category.Event.event_date
            });

            return Ok(response);
        }

        /// <summary>
        /// Allows an organiser to view results for their own event.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var organiser = await GetCurrentOrganiserAsync(userId);

            if (organiser == null)
            {
                return Forbid();
            }

            bool ownsEvent = await _context.Events
                .AnyAsync(e =>
                    e.event_id == eventId &&
                    e.organiser_id == organiser.organiser_id);

            if (!ownsEvent)
            {
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });
            }

            var results = await _context.Results
                .AsNoTracking()
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Participant)
                        .ThenInclude(p => p!.User)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e!.Category)
                .Where(r =>
                    r.Enrolment!.Category!.event_id == eventId)
                .OrderBy(r => r.finishing_position)
                .ToListAsync();

            var response = results.Select(r => new
            {
                r.result_id,
                r.finish_time,
                r.finishing_position,
                participantId =
                    r.Enrolment!.Participant!.participant_id,
                firstName =
                    r.Enrolment.Participant.User!.first_name,
                lastName =
                    r.Enrolment.Participant.User.last_name,
                categoryName =
                    r.Enrolment.Category!.category_name
            });

            return Ok(response);
        }
    }
}