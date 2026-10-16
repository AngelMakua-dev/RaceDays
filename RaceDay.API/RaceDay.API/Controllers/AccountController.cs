using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// Returns the identity and role of the authenticated user.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            string? userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            string? email = User.FindFirstValue(
                ClaimTypes.Email);

            string? role = User.FindFirstValue(
                ClaimTypes.Role);

            return Ok(new
            {
                message = "You are authenticated.",
                userId,
                email,
                role
            });
        }

        /// <summary>
        /// Example endpoint accessible only to organisers.
        /// </summary>
        [Authorize(Roles = "Organiser")]
        [HttpGet("organiser-only")]
        public IActionResult OrganiserOnly()
        {
            return Ok(new
            {
                message = "Access granted to an organiser."
            });
        }

        /// <summary>
        /// Example endpoint accessible only to participants.
        /// </summary>
        [Authorize(Roles = "Participant")]
        [HttpGet("participant-only")]
        public IActionResult ParticipantOnly()
        {
            return Ok(new
            {
                message = "Access granted to a participant."
            });
        }
    }
}