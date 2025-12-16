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
        /// Authentifie un utilisateur et initialise une session sécurisée via HttpOnly Cookie.
        /// </summary>
        /// <param name="loginDto">Les informations de connexion (Email/Pseudo/Tel et Mot de passe).</param>
        /// <returns>Un objet AuthResponseDto contenant les infos utilisateur (sans le token, car stocké dans le cookie).</returns>
        /// <response code="200">Authentification réussie, cookie défini.</response>
        /// <response code="400">Données d'entrée invalides.</response>
        /// <response code="401">Identifiant ou mot de passe incorrect.</response>
        /// <response code="500">Erreur serveur (clé JWT manquante).</response>
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
                    && u.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid identifier or password.");
            }

            // AJOUT : Gestion de l'avatar dans les claims pour le récupérer dans le endpoint /Me
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRoleNavigation.Label),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("AvatarPath", user.AvatarUrl ?? "") // Claim personnalisée
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
                AvatarPath = user.AvatarUrl
            };

            return Ok(response);
        }

        /// <summary>
        /// Déconnecte l'utilisateur en supprimant le cookie d'authentification.
        /// </summary>
        /// <returns>Un message de confirmation.</returns>
        /// <response code="200">Déconnexion réussie.</response>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");
            return Ok(new { message = "Déconnexion réussie" });
        }

        /// <summary>
        /// Récupère les informations de l'utilisateur actuellement connecté via le cookie de session.
        /// </summary>
        /// <returns>Les informations de l'utilisateur courant.</returns>
        /// <response code="200">Utilisateur authentifié trouvé.</response>
        /// <response code="401">Utilisateur non authentifié.</response>
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
                AvatarPath = avatarUrl
            });
        }
    }
}