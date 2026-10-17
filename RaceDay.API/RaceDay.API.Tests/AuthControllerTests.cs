using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Tests
{
    public class AuthControllerTests
    {
        private RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private IConfiguration CreateConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] =
                    "RaceDayDevelopmentKey2026_SecureKey_TestOnly!",
                ["Jwt:Issuer"] = "RaceDay.API",
                ["Jwt:Audience"] = "RaceDay.Client",
                ["Jwt:ExpiryMinutes"] = "60"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task Register_ValidParticipant_ReturnsCreated()
        {
            await using var context = CreateContext();

            var controller = new AuthController(
                context,
                CreateConfiguration());

            var request = new RegisterRequest
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "testparticipant@example.com",
                Password = "Password123!",
                Role = "Participant",
                DateOfBirth = new DateTime(2000, 5, 15),
                EmergencyContactName = "Emergency Contact",
                EmergencyContactPhone = "0712345678"
            };

            var result = await controller.Register(request);

            var createdResult =
                Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, createdResult.StatusCode);

            Assert.Single(context.Users);
            Assert.Single(context.Participants);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            await using var context = CreateContext();

            var configuration = CreateConfiguration();

            var controller = new AuthController(
                context,
                configuration);

            var registerRequest = new RegisterRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "CorrectPassword123!",
                Role = "Participant",
                DateOfBirth = new DateTime(2000, 5, 15),
                EmergencyContactName = "Emergency Contact",
                EmergencyContactPhone = "0712345678"
            };

            await controller.Register(registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "testuser@example.com",
                Password = "WrongPassword123!"
            };

            var result = await controller.Login(loginRequest);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}