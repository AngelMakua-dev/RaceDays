using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Tests
{
    public class EventControllerTests
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
        public async Task Organiser_CanCreateEvent()
        {
            await using var context = CreateContext();

            var organiserUser = new User
            {
                user_id = 1,
                first_name = "Test",
                last_name = "Organiser",
                email = "organiser@example.com",
                password = "hashed-password",
                role = "Organiser",
                created_at = DateTime.Now
            };

            context.Users.Add(organiserUser);

            var organiser = new Organiser
            {
                organiser_id = 1,
                user_id = 1,
                organisation_name = "RaceDay Test Organisation"
            };

            context.Organisers.Add(organiser);

            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            SetUser(
                controller,
                1,
                "organiser@example.com",
                "Organiser");

            var request = new EventRequest
            {
                EventName = "Polokwane Test Race",
                Description = "Test RaceDay event",
                EventDate = DateTime.Today.AddDays(30),
                Location = "Polokwane",
                DistanceKm = 10,
                EventType = "Run",
                Status = "Upcoming"
            };

            var result = await controller.CreateEvent(request);

            var createdResult =
                Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(201, createdResult.StatusCode);

            Assert.Single(context.Events);

            var savedEvent = await context.Events.FirstAsync();

            Assert.Equal(
                "Polokwane Test Race",
                savedEvent.event_name);

            Assert.Equal(
                10,
                savedEvent.distance_km);

            Assert.Equal(
                "Run",
                savedEvent.event_type);
        }

        [Fact]
        public async Task Organiser_CannotCreatePastEvent()
        {
            await using var context = CreateContext();

            var user = new User
            {
                user_id = 1,
                first_name = "Test",
                last_name = "Organiser",
                email = "organiser2@example.com",
                password = "hashed-password",
                role = "Organiser",
                created_at = DateTime.Now
            };

            context.Users.Add(user);

            context.Organisers.Add(new Organiser
            {
                organiser_id = 1,
                user_id = 1,
                organisation_name = "Test Organisation"
            });

            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            SetUser(
                controller,
                1,
                "organiser2@example.com",
                "Organiser");

            var request = new EventRequest
            {
                EventName = "Past Race",
                Description = "This should fail",
                EventDate = DateTime.Today.AddDays(-1),
                Location = "Polokwane",
                DistanceKm = 10,
                EventType = "Run",
                Status = "Upcoming"
            };

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);

            Assert.Empty(context.Events);
        }
    }
}