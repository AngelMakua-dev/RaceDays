using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;

        public AuthController(
            RaceDayDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                    request.DateOfBirth.Value.Date >= DateTime.Today)
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

            // Store a password hash, never the plain-text password.
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

        /// <summary>
        /// Authenticates a registered user and returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.password,
                request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            string jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is missing.");

            string issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is missing.");

            string audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is missing.");

            int expiryMinutes = int.TryParse(
                _configuration["Jwt:ExpiryMinutes"],
                out int configuredExpiry)
                    ? configuredExpiry
                    : 60;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                new Claim(ClaimTypes.Name, user.email),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            string tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            var response = new LoginResponse
            {
                Message = "Login successful.",
                UserId = user.user_id,
                FirstName = user.first_name,
                LastName = user.last_name,
                Email = user.email,
                Role = user.role,
                Token = tokenString
            };

            return Ok(response);
        }
    }
}