using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<NZWalksUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<NZWalksUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // POST: api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if user already exists
                var existingUser = await userManager.FindByNameAsync(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                var existingEmail = await userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                // Create new user
                var newUser = new NZWalksUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                // Create user with password
                var createResult = await userManager.CreateAsync(newUser, request.Password);

                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(new { message = "User registration failed", errors });
                }

                // Add roles to user
                if (request.Roles != null && request.Roles.Length > 0)
                {
                    foreach (var role in request.Roles)
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                    }
                }
                else
                {
                    // Default role: Reader
                    await userManager.AddToRoleAsync(newUser, "Reader");
                }

                return Ok(new { message = "User registered successfully", userId = newUser.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Find user by username
                var user = await userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid username or password" });
                }

                // Check password
                var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

                if (!isPasswordValid)
                {
                    return BadRequest(new { message = "Invalid username or password" });
                }

                // Get user roles
                var roles = await userManager.GetRolesAsync(user);

                // Generate JWT token
                var token = tokenRepository.CreateJwtToken(user.UserName, user.Email, roles.ToList());

                var response = new LoginResponseDto
                {
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }
    }
}
