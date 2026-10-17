
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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        private async Task<Participant?> GetParticipantAsync(int userId)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(p => p.user_id == userId);
        }

        private async Task<Organiser?> GetOrganiserAsync(int userId)
        {
            return await _context.Organisers
                .FirstOrDefaultAsync(o => o.user_id == userId);
        }

        /// <summary>
        /// Enrols the authenticated participant in a category.
        /// </summary>
        [Authorize(Roles = "Participant")]
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] EnrolmentRequest request)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var participant = await GetParticipantAsync(userId);

            if (participant == null)
            {
                return NotFound(new
                {
                    message = "Participant profile not found."
                });
            }

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c =>
                    c.category_id == request.CategoryId);

            if (category == null || category.Event == null)
            {
                return NotFound(new
                {
                    message = "Category or associated event not found."
                });
            }

            if (category.Event.event_date.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "You cannot enrol in an event that has already passed."
                });
            }

            bool alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.participant_id == participant.participant_id &&
                    e.category_id == category.category_id);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "You are already enrolled in this category."
                });
            }

            var enrolment = new Enrolment
            {
                participant_id = participant.participant_id,
                category_id = category.category_id,
                enrolment_date = DateTime.Now,
                status = "Registered"
            };

            _context.Enrolments.Add(enrolment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Also handles a duplicate rejected by the database constraint.
                return Conflict(new
                {
                    message = "The enrolment could not be saved. Check whether you are already enrolled."
                });
            }

            return StatusCode(201, new
            {
                message = "Enrolment created successfully.",
                enrolment.enrolment_id,
                enrolment.participant_id,
                enrolment.category_id,
                category.category_name,
                eventName = category.Event.event_name,
                enrolment.enrolment_date,
                enrolment.status
            });
        }

        /// <summary>
        /// Lists enrolments belonging to the authenticated participant.
        /// </summary>
        [Authorize(Roles = "Participant")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var participant = await GetParticipantAsync(userId);

            if (participant == null)
            {
                return NotFound(new
                {
                    message = "Participant profile not found."
                });
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.participant_id == participant.participant_id)
                .Include(e => e.Category)
                    .ThenInclude(c => c!.Event)
                .OrderByDescending(e => e.enrolment_date)
                .ToListAsync();

            var response = enrolments.Select(e => new
            {
                e.enrolment_id,
                e.status,
                e.enrolment_date,
                e.category_id,
                categoryName = e.Category!.category_name,
                eventId = e.Category.Event!.event_id,
                eventName = e.Category.Event.event_name,
                eventDate = e.Category.Event.event_date,
                location = e.Category.Event.location
            });

            return Ok(response);
        }

        /// <summary>
        /// Lists enrolments for an event owned by the authenticated organiser.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var organiser = await GetOrganiserAsync(userId);

            if (organiser == null)
                return Forbid();

            bool ownsEvent = await _context.Events.AnyAsync(e =>
                e.event_id == eventId &&
                e.organiser_id == organiser.organiser_id);

            if (!ownsEvent)
            {
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.Category!.event_id == eventId)
                .Include(e => e.Participant)
                    .ThenInclude(p => p!.User)
                .Include(e => e.Category)
                .OrderBy(e => e.category_id)
                .ToListAsync();

            var response = enrolments.Select(e => new
            {
                e.enrolment_id,
                e.status,
                e.enrolment_date,
                categoryName = e.Category!.category_name,
                participantId = e.Participant!.participant_id,
                firstName = e.Participant.User!.first_name,
                lastName = e.Participant.User.last_name,
                email = e.Participant.User.email
            });

            return Ok(response);
        }
    }
}