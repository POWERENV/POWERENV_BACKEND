using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using POWERENV_PGSQL_DB_HANDLER;
using System.Security.Claims;
using static POWERENV_DB_HANDLER.USER_DATA_HANDLING.USER_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/backend/user/auth")]
    public class USER_AUTH_CONTROLLER : Controller
    {
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public USER_AUTH_CONTROLLER()
        {
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            var validatedUser = await ValidateUserCredentialsAsync(request.Email, request.Password);

            if (validatedUser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Generate Identity ticket metadata (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, validatedUser.user_id.ToString()),
                new Claim(ClaimTypes.Name, $"{validatedUser.user_first_name} {validatedUser.user_last_name}"),
                new Claim(ClaimTypes.Email, validatedUser.user_email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            // Issue cookie and store underlying ticket metadata inside Redis distributed memory
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            response.operationStatus = true;
            response.statusMessage = "Login successful. Session backed by Redis.";
            response.packetData = new
            {
                userId = validatedUser.user_id,
                username = $"{validatedUser.user_first_name} {validatedUser.user_last_name}",
                email = validatedUser.user_email
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            // Deletes the browser cookie and invalidates the session cache track
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            response.operationStatus = true;
            response.statusMessage = "Logged out successfully!";

            return Ok(response);
        }

        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                response.operationStatus = true;
                response.statusMessage = "User is authenticated.";
                response.packetData = new
                {
                    userId,
                    username,
                    email
                };
            }
            else
            {
                response.operationStatus = false;
                response.statusMessage = "User is not authenticated.";
            }

            return Ok(response);
        }

        private async Task<UserProfileInfo?> ValidateUserCredentialsAsync(string email, string plainPassword)
        {
            UserProfileInfo userProfileInfo = DB_HANDLER.USER_DATA_HANDLER.DBValidateUsername(email);

            if (userProfileInfo == null)
            {
                return null; // User not found
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, userProfileInfo.user_password_hash);

            if (isPasswordValid) return userProfileInfo;

            return null;
        }
    }
}