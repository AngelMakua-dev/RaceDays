
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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        private async Task<Organiser?> GetCurrentOrganiserAsync()
        {
            string? idText =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idText, out int userId))
                return null;

            return await _context.Organisers
                .FirstOrDefaultAsync(o => o.user_id == userId);
        }

        private static object ToResponse(Category c)
        {
            return new
            {
                c.category_id,
                c.event_id,
                c.category_name,
                c.distance_km,
                c.entry_fee
            };
        }

        /// <summary>
        /// Lists categories, optionally filtered by event ID.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories(
            [FromQuery] int? eventId)
        {
            var query = _context.Categories
                .AsNoTracking()
                .AsQueryable();

            if (eventId.HasValue)
                query = query.Where(c => c.event_id == eventId.Value);

            var categories = await query
                .OrderBy(c => c.event_id)
                .ThenBy(c => c.category_name)
                .ToListAsync();

            return Ok(categories.Select(ToResponse));
        }

        /// <summary>
        /// Gets one category by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.category_id == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            return Ok(ToResponse(category));
        }

        /// <summary>
        /// Creates a category for an event owned by the organiser.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpPost("event/{eventId:int}")]
        public async Task<IActionResult> CreateCategory(
            int eventId,
            [FromBody] CategoryRequest request)
        {
            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
                return Forbid();

            var eventExists = await _context.Events
                .AnyAsync(e => e.event_id == eventId &&
                               e.organiser_id == organiser.organiser_id);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found or you do not own it."
                });
            }

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.event_id == eventId &&
                c.category_name == request.CategoryName.Trim());

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "This category already exists for the event."
                });
            }

            var category = new Category
            {
                event_id = eventId,
                category_name = request.CategoryName.Trim(),
                distance_km = request.DistanceKm,
                entry_fee = request.EntryFee
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.category_id },
                ToResponse(category));
        }

        /// <summary>
        /// Updates a category belonging to an event owned by the organiser.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] CategoryRequest request)
        {
            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.category_id == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (category.Event == null ||
                category.Event.organiser_id != organiser.organiser_id)
                return Forbid();

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.event_id == category.event_id &&
                c.category_id != id &&
                c.category_name == request.CategoryName.Trim());

            if (duplicate)
            {
                return Conflict(new
                {
                    message = "This category name already exists for the event."
                });
            }

            category.category_name = request.CategoryName.Trim();
            category.distance_km = request.DistanceKm;
            category.entry_fee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(ToResponse(category));
        }

        /// <summary>
        /// Deletes a category belonging to an event owned by the organiser.
        /// Categories with enrolments cannot be deleted.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var organiser = await GetCurrentOrganiserAsync();

            if (organiser == null)
                return Forbid();

            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.category_id == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (category.Event == null ||
                category.Event.organiser_id != organiser.organiser_id)
                return Forbid();

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.category_id == id);

            if (hasEnrolments)
            {
                return Conflict(new
                {
                    message = "This category has enrolments and cannot be deleted."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}