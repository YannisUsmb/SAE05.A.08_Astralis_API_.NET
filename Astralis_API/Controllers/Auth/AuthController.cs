using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AstralisDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AstralisDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user and initializes a secure session via an HttpOnly Cookie.
        /// </summary>
        /// <param name="loginDto">The login credentials (Email/Username/Phone and Password).</param>
        /// <returns>An AuthResponseDto containing user info (excluding the token, stored in the cookie).</returns>
        /// <response code="200">Authentication successful, cookie set.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Invalid identifier or password.</response>
        /// <response code="500">Server error (JWT Key missing).</response>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User? user = await _context.Users
                .Include(u => u.UserRoleNavigation)
                .FirstOrDefaultAsync(u =>
                    (u.Email == loginDto.Identifier ||
                     u.Username == loginDto.Identifier ||
                     u.Phone == loginDto.Identifier)
                    && u.Password == BCrypt.Net.BCrypt.HashPassword(loginDto.Password));

            if (user == null)
            {
                return Unauthorized("Invalid identifier or password.");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRoleNavigation.Label),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("AvatarPath", user.AvatarUrl ?? ""),
                new Claim("IsPremium", user.IsPremium ? "true" : "false")
            };

            string? keyString = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                return StatusCode(500, "JWT Key is not configured.");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime expiresAt = DateTime.Now.AddHours(4);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt
            };

            Response.Cookies.Append("authToken", jwt, cookieOptions);

            AuthResponseDto response = new AuthResponseDto
            {
                Token = jwt,
                Expiration = expiresAt,
                Username = user.Username,
                Role = user.UserRoleNavigation.Label,
                AvatarPath = user.AvatarUrl,
                IsPremium = user.IsPremium
            };

            return Ok(response);
        }

        /// <summary>
        /// Logs the user out by deleting the authentication cookie.
        /// </summary>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">Logout successful.</response>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");
            return Ok(new { message = "Logout successful" });
        }

        /// <summary>
        /// Retrieves the currently authenticated user's information via the session cookie.
        /// </summary>
        /// <returns>The current user's details.</returns>
        /// <response code="200">Authenticated user found.</response>
        /// <response code="401">User not authenticated.</response>
        [HttpGet("Me")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<AuthResponseDto> GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var avatarUrl = User.FindFirst("AvatarPath")?.Value;

            return Ok(new AuthResponseDto
            {
                Username = username,
                Role = role,
                Token = "",
                Expiration = DateTime.Now.AddHours(1),
                AvatarPath = avatarUrl,
                IsPremium = User.FindFirst("IsPremium")?.Value == "true"
            });
        }
    }
}