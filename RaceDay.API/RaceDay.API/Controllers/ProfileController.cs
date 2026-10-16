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
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        /// <summary>
        /// Gets the authenticated user's profile.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.user_id == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User profile not found."
                });
            }

            var response = new ProfileResponse
            {
                UserId = user.user_id,
                FirstName = user.first_name,
                LastName = user.last_name,
                Email = user.email,
                Role = user.role
            };

            if (user.role == "Organiser")
            {
                var organiser = await _context.Organisers
                    .FirstOrDefaultAsync(o => o.user_id == userId);

                response.OrganisationName =
                    organiser?.organisation_name;
            }
            else if (user.role == "Participant")
            {
                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.user_id == userId);

                if (participant != null)
                {
                    response.DateOfBirth =
                        participant.date_of_birth;

                    response.EmergencyContactName =
                        participant.emergency_contact_name;

                    response.EmergencyContactPhone =
                        participant.emergency_contact_phone;
                }
            }

            return Ok(response);
        }

        /// <summary>
        /// Updates the authenticated user's profile.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.user_id == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User profile not found."
                });
            }

            string email = request.Email.Trim().ToLowerInvariant();

            bool emailExists = await _context.Users.AnyAsync(
                u => u.email == email && u.user_id != userId);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "That email address is already in use."
                });
            }

            user.first_name = request.FirstName.Trim();
            user.last_name = request.LastName.Trim();
            user.email = email;

            if (user.role == "Organiser")
            {
                if (string.IsNullOrWhiteSpace(request.OrganisationName))
                {
                    return BadRequest(new
                    {
                        message = "Organisation name is required."
                    });
                }

                var organiser = await _context.Organisers
                    .FirstOrDefaultAsync(o => o.user_id == userId);

                if (organiser == null)
                {
                    return NotFound(new
                    {
                        message = "Organiser profile not found."
                    });
                }

                organiser.organisation_name =
                    request.OrganisationName.Trim();
            }
            else if (user.role == "Participant")
            {
                if (request.DateOfBirth == null ||
                    request.DateOfBirth.Value.Date >= DateTime.Today)
                {
                    return BadRequest(new
                    {
                        message = "A valid date of birth is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(
                        request.EmergencyContactName) ||
                    string.IsNullOrWhiteSpace(
                        request.EmergencyContactPhone))
                {
                    return BadRequest(new
                    {
                        message = "Emergency contact details are required."
                    });
                }

                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.user_id == userId);

                if (participant == null)
                {
                    return NotFound(new
                    {
                        message = "Participant profile not found."
                    });
                }

                participant.date_of_birth =
                    request.DateOfBirth.Value.Date;

                participant.emergency_contact_name =
                    request.EmergencyContactName.Trim();

                participant.emergency_contact_phone =
                    request.EmergencyContactPhone.Trim();
            }
            else
            {
                return BadRequest(new
                {
                    message = "The account has an unsupported role."
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }
    }
}