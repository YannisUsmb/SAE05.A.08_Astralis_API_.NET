using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
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
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User credentials (email/username/phone and password).</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">Returns the token.</response>
        /// <response code="401">Invalid credentials.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User? user = await _context.Users
                .Include(u => u.UserRoleNavigation)
                .FirstOrDefaultAsync(u =>
                    (u.Email == loginDto.Identifier ||
                     u.Username == loginDto.Identifier ||
                     u.Phone == loginDto.Identifier)
                    && u.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid identifier or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRoleNavigation.Label),
                new Claim(ClaimTypes.Name, user.Username)
            };

            string? keyString = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(keyString))
                return StatusCode(500, "JWT Key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwt });
        }
    }
}