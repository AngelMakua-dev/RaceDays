using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Tests
{
    public class RoleAccessTests
    {
        private RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private static void SetUser(
            ControllerBase controller,
            int userId,
            string email,
            string role)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            var identity = new ClaimsIdentity(
                claims,
                "TestAuthentication");

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(identity)
                    }
                };
        }

        [Fact]
        public async Task Participant_CannotCreateEvent()
        {
            await using var context = CreateContext();

            var controller = new EventsController(context);

            SetUser(
                controller,
                1,
                "participant@example.com",
                "Participant");

            var request = new RaceDay.API.DTOs.EventRequest
            {
                EventName = "Test Race",
                Description = "Test event",
                EventDate = DateTime.Today.AddDays(30),
                Location = "Polokwane",
                DistanceKm = 10,
                EventType = "Run",
                Status = "Upcoming"
            };

            // The controller method itself does not perform
            // ASP.NET's [Authorize] filter.
            //
            // Therefore, we verify the role restriction
            // by checking that the endpoint is decorated
            // with [Authorize(Roles = "Organiser")].

            var method = typeof(EventsController)
                .GetMethod(nameof(EventsController.CreateEvent));

            Assert.NotNull(method);

            var attributes = method!
                .GetCustomAttributes(
                    typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute),
                    true);

            var authorizeAttribute =
                attributes
                    .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .FirstOrDefault();

            Assert.NotNull(authorizeAttribute);

            Assert.Equal(
                "Organiser",
                authorizeAttribute!.Roles);
        }

        [Fact]
        public void ParticipantOnlyEndpoint_RequiresParticipantRole()
        {
            var method = typeof(AccountController)
                .GetMethod(nameof(AccountController.ParticipantOnly));

            Assert.NotNull(method);

            var attributes = method!
                .GetCustomAttributes(
                    typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute),
                    true);

            var authorizeAttribute =
                attributes
                    .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .FirstOrDefault();

            Assert.NotNull(authorizeAttribute);

            Assert.Equal(
                "Participant",
                authorizeAttribute!.Roles);
        }

        [Fact]
        public void OrganiserOnlyEndpoint_RequiresOrganiserRole()
        {
            var method = typeof(AccountController)
                .GetMethod(nameof(AccountController.OrganiserOnly));

            Assert.NotNull(method);

            var attributes = method!
                .GetCustomAttributes(
                    typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute),
                    true);

            var authorizeAttribute =
                attributes
                    .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
                    .FirstOrDefault();

            Assert.NotNull(authorizeAttribute);

            Assert.Equal(
                "Organiser",
                authorizeAttribute!.Roles);
        }
    }
}