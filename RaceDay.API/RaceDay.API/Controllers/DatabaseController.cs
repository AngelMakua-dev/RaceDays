using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public DatabaseController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                bool connected = await _context.Database.CanConnectAsync();

                if (!connected)
                {
                    return StatusCode(503, new
                    {
                        message = "Could not connect to the RaceDay database.",
                        connected = false
                    });
                }

                return Ok(new
                {
                    message = "Successfully connected to SQL Server.",
                    database = "RaceDay",
                    server = "localhost",
                    connected = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    message = "Database connection failed.",
                    error = ex.Message,
                    connected = false
                });
            }
        }
    }
}