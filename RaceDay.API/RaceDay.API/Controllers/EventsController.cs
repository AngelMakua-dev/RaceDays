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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        private async Task<Organiser?> GetCurrentOrganiserAsync()
        {
            string? userIdText =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdText, out int userId))
            {
                return null;
            }

            return await _context.Organisers
                .FirstOrDefaultAsync(o => o.user_id == userId);
        }

        private static object ToEventResponse(Event e)
        {
            return new
            {
                e.event_id,
                e.organiser_id,
                e.event_name,
                e.description,
                e.event_date,
                e.location,
                e.distance_km,
                e.event_type,
                e.status
            };
        }

        /// <summary>
        /// Lists all events for authenticated users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.event_date)
                .ToListAsync();

            return Ok(events.Select(ToEventResponse));
        }

        /// <summary>
        /// Gets one event by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var e = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.event_id == id);

            if (e == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(ToEventResponse(e));
        }

        /// <summary>
        /// Creates an event. Organisers only.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventRequest request)
        {
            if (request.EventDate.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "The event date cannot be in the past."
                });
            }

            if (!IsValidStatus(request.Status))
            {
                return BadRequest(new
                {
                    message = "Status must be Upcoming, Ongoing, or Completed."
                });
            }

            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
            {
                return Forbid();
            }

            var e = new Event
            {
                organiser_id = organiser.organiser_id,
                event_name = request.EventName.Trim(),
                description = request.Description.Trim(),
                event_date = request.EventDate,
                location = request.Location.Trim(),
                distance_km = request.DistanceKm,
                event_type = request.EventType,
                status = request.Status
            };

            _context.Events.Add(e);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = e.event_id },
                ToEventResponse(e));
        }

        /// <summary>
        /// Updates an event owned by the current organiser.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventRequest request)
        {
            if (request.EventDate.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "The event date cannot be in the past."
                });
            }

            if (!IsValidStatus(request.Status))
            {
                return BadRequest(new
                {
                    message = "Status must be Upcoming, Ongoing, or Completed."
                });
            }

            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
            {
                return Forbid();
            }

            var e = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == id);

            if (e == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (e.organiser_id != organiser.organiser_id)
            {
                return Forbid();
            }

            e.event_name = request.EventName.Trim();
            e.description = request.Description.Trim();
            e.event_date = request.EventDate;
            e.location = request.Location.Trim();
            e.distance_km = request.DistanceKm;
            e.event_type = request.EventType;
            e.status = request.Status;

            await _context.SaveChangesAsync();

            return Ok(ToEventResponse(e));
        }

        /// <summary>
        /// Deletes an event owned by the current organiser.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
            {
                return Forbid();
            }

            var e = await _context.Events
                .FirstOrDefaultAsync(e => e.event_id == id);

            if (e == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            if (e.organiser_id != organiser.organiser_id)
            {
                return Forbid();
            }

            bool hasEnrolments = await _context.Categories
                .AnyAsync(c => c.event_id == id &&
                    c.Enrolments.Any());

            if (hasEnrolments)
            {
                return Conflict(new
                {
                    message = "This event has enrolments. It cannot be deleted until those enrolments are handled."
                });
            }

            bool hasCategories = await _context.Categories
                .AnyAsync(c => c.event_id == id);

            if (hasCategories)
            {
                return Conflict(new
                {
                    message = "Delete the event's categories first."
                });
            }

            _context.Events.Remove(e);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }

        private static bool IsValidStatus(string? status)
        {
            return status is "Upcoming" or "Ongoing" or "Completed";
        }
    }
}