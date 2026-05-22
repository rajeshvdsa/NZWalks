using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        // Test endpoint - NO authentication required
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { message = "API is running", timestamp = DateTime.UtcNow });
        }

        // Test endpoint - Authentication required
        [HttpGet("claims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var identity = User.Identity;

            return Ok(new
            {
                isAuthenticated = identity?.IsAuthenticated,
                userName = identity?.Name,
                claims = claims
            });
        }

        // Test endpoint - Check if authorized
        [HttpGet("check-role")]
        [Authorize]
        public IActionResult CheckRole()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var isReader = User.IsInRole("Reader");
            var isWriter = User.IsInRole("Writer");

            return Ok(new
            {
                roleClaim,
                isReader,
                isWriter,
                allRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }
    }
}
