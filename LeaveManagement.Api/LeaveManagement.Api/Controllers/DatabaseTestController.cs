using LeaveManagement.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/db-test")]
    [ApiController]
    public class DatabaseTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new { coonected = canConnect });
        }
    }
}
