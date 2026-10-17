
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
    public class EnrolmentControllerTests
    {
        private RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private static void SetParticipant(
            ControllerBase controller,
            int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),
                new Claim(
                    ClaimTypes.Role,
                    "Participant")
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, "Test"))
                }
            };
        }

        private static async Task SeedParticipantAndCategory(
            RaceDayDbContext context)
        {
            context.Users.Add(new User
            {
                user_id = 1,
                first_name = "Test",
                last_name = "Participant",
                email = "participant@example.com",
                password = "test-hash",
                role = "Participant",
                created_at = DateTime.Now
            });

            context.Participants.Add(new Participant
            {
                participant_id = 1,
                user_id = 1,
                date_of_birth = new DateTime(2000, 1, 1),
                emergency_contact_name = "Emergency Contact",
                emergency_contact_phone = "0712345678"
            });

            context.Users.Add(new User
            {
                user_id = 2,
                first_name = "Test",
                last_name = "Organiser",
                email = "organiser@example.com",
                password = "test-hash",
                role = "Organiser",
                created_at = DateTime.Now
            });

            context.Organisers.Add(new Organiser
            {
                organiser_id = 1,
                user_id = 2,
                organisation_name = "Test Organisation"
            });

            context.Events.Add(new Event
            {
                event_id = 1,
                organiser_id = 1,
                event_name = "Test Race",
                event_date = DateTime.Today.AddDays(30),
                location = "Polokwane",
                description = "Test event",
                distance_km = 10,
                event_type = "Run",
                status = "Upcoming"
            });

            context.Categories.Add(new Category
            {
                category_id = 1,
                event_id = 1,
                category_name = "10 km Run",
                distance_km = 10,
                entry_fee = 50
            });

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Participant_CanEnrolInCategory()
        {
            await using var context = CreateContext();

            await SeedParticipantAndCategory(context);

            var controller = new EnrolmentsController(context);
            SetParticipant(controller, 1);

            var request = new EnrolmentRequest
            {
                CategoryId = 1
            };

            var result = await controller.CreateEnrolment(request);

            var created = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, created.StatusCode);
            Assert.Single(context.Enrolments);
        }

        [Fact]
        public async Task Participant_CannotEnrolTwiceInSameCategory()
        {
            await using var context = CreateContext();

            await SeedParticipantAndCategory(context);

            context.Enrolments.Add(new Enrolment
            {
                enrolment_id = 1,
                participant_id = 1,
                category_id = 1,
                enrolment_date = DateTime.Now,
                status = "Registered"
            });

            await context.SaveChangesAsync();

            var controller = new EnrolmentsController(context);
            SetParticipant(controller, 1);

            var request = new EnrolmentRequest
            {
                CategoryId = 1
            };

            var result = await controller.CreateEnrolment(request);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Single(context.Enrolments);
        }
    }
}