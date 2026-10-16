
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        /// <summary>
        /// Registers a new organiser or participant.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            string role = request.Role.Trim();

            if (role != "Organiser" && role != "Participant")
            {
                return BadRequest(new
                {
                    message = "Role must be Organiser or Participant."
                });
            }

            string email = request.Email.Trim().ToLowerInvariant();

            bool emailExists = await _context.Users
                .AnyAsync(u => u.email == email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "An account with this email already exists."
                });
            }

            if (role == "Organiser" &&
                string.IsNullOrWhiteSpace(request.OrganisationName))
            {
                return BadRequest(new
                {
                    message = "Organisation name is required for organisers."
                });
            }

            if (role == "Participant")
            {
                if (request.DateOfBirth == null ||
                    request.DateOfBirth.Value.Date >= DateTime.UtcNow.Date)
                {
                    return BadRequest(new
                    {
                        message = "A valid date of birth is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.EmergencyContactName) ||
                    string.IsNullOrWhiteSpace(request.EmergencyContactPhone))
                {
                    return BadRequest(new
                    {
                        message = "Emergency contact name and phone are required."
                    });
                }
            }

            var user = new User
            {
                first_name = request.FirstName.Trim(),
                last_name = request.LastName.Trim(),
                email = email,
                role = role,
                created_at = DateTime.Now
            };

            user.password = _passwordHasher.HashPassword(
                user,
                request.Password);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (role == "Organiser")
                {
                    var organiser = new Organiser
                    {
                        user_id = user.user_id,
                        organisation_name =
                            request.OrganisationName!.Trim()
                    };

                    _context.Organisers.Add(organiser);
                }
                else
                {
                    var participant = new Participant
                    {
                        user_id = user.user_id,
                        date_of_birth =
                            request.DateOfBirth!.Value.Date,
                        emergency_contact_name =
                            request.EmergencyContactName!.Trim(),
                        emergency_contact_phone =
                            request.EmergencyContactPhone!.Trim()
                    };

                    _context.Participants.Add(participant);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return StatusCode(201, new
                {
                    message = "Account registered successfully.",
                    userId = user.user_id,
                    email = user.email,
                    role = user.role
                });
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return Conflict(new
                {
                    message = "The account could not be saved. Check whether the email already exists."
                });
            }
        }
    }
}