using Astralis.Shared.DTOs;
using Astralis.Shared.Enums;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using Astralis_API.Services.Interfaces;
using Google.Apis.Auth;
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

        private readonly IUserRepository _userRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IEmailService _emailService;

        public AuthController(AstralisDbContext context, IConfiguration configuration, IUserRepository userRepository, ICountryRepository countryRepository, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            this._emailService = emailService;
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
        public async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? user = null;

            if (!string.IsNullOrWhiteSpace(loginDto.Phone) && loginDto.CountryId.HasValue)
            {
                var country = await _countryRepository.GetByIdAsync(loginDto.CountryId.Value);
                if (country == null)
                    return BadRequest("Pays invalide.");

                user = await _userRepository.GetByPhoneAndPrefixAsync(loginDto.Phone, country.PhonePrefixId);
            }
            else if (!string.IsNullOrWhiteSpace(loginDto.Identifier))
            {
                user = await _userRepository.GetByEmailOrUsernameAsync(loginDto.Identifier);
            }

            if (user == null)
            {
                return Unauthorized("Identifiant ou mot de passe incorrect.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Identifiant ou mot de passe incorrect.");
            }

            if (!user.IsEmailVerified)
            {
                return Unauthorized("Votre compte n'est pas activé. Veuillez vérifier vos emails.");
            }

            return GenerateSession(user);
        }

        /// <summary>
        /// Authenticates a user via Google OAuth2 token. Creates a new account if the email doesn't exist.
        /// </summary>
        /// <param name="googleDto">The Google ID Token received from the client.</param>
        /// <returns>An AuthResponseDto containing user info.</returns>
        /// <response code="200">Authentication successful, cookie set.</response>
        /// <response code="400">Invalid Google Token.</response>
        /// <response code="500">Server error.</response>
        [HttpPost("GoogleLogin")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginDto googleDto)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleDto.IdToken, settings);

                var user = await _context.Users
                    .Include(u => u.UserRoleNavigation)
                    .FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        FirstName = payload.GivenName ?? "Explorateur",
                        LastName = payload.FamilyName ?? "Astralis",
                        Username = (payload.GivenName ?? "User") + new Random().Next(1000, 9999),
                        AvatarUrl = payload.Picture,
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                        UserRoleId = 1,
                        InscriptionDate = DateOnly.FromDateTime(DateTime.Now),
                        IsPremium = false,
                        PhonePrefixId = null,
                        MultiFactorAuthentification = false,
                        Gender = GenderType.Unknown 
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await _context.Entry(user).Reference(u => u.UserRoleNavigation).LoadAsync();
                }

                return GenerateSession(user);
            }
            catch (InvalidJwtException)
            {
                return BadRequest("Token Google invalide.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Erreur interne lors de la connexion Google.");
            }
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
        public async Task<ActionResult<AuthResponseDto>> GetCurrentUser()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user != null)
                {
                    return Ok(new AuthResponseDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Role = User.FindFirst(ClaimTypes.Role)?.Value ?? user.UserRoleNavigation?.Label ?? "Membre",
                        Token = "",
                        Expiration = DateTime.Now.AddHours(1),
                        AvatarUrl = user.AvatarUrl,
                        IsPremium = user.IsPremium
                    });
                }
            }

            return Unauthorized();
        }

        [HttpPost("Verify-Email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null)
            {
                return BadRequest("Jeton invalide.");
            }

            if (user.TokenExpiration < DateTime.Now)
            {
                return BadRequest("Le lien a expiré.");
            }

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.TokenExpiration = null;

            await _context.SaveChangesAsync();

            return Ok("Email vérifié avec succès !");
        }

        /// <summary>
        /// Sends a password reset link to the user's email if it exists.
        /// </summary>
        [HttpPost("Forgot-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(dto.Email);

            if (user == null)
            {
                return Ok(new { message = "Si cet email existe, un lien a été envoyé." });
            }

            user.VerificationToken = Guid.NewGuid().ToString();
            user.TokenExpiration = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var resetLink = $"https://localhost:7036/reset-password?token={user.VerificationToken}";

            try
            {
                string subject = "Réinitialisation de votre mot de passe";
                string message = $@"
                    <div style='background: #0f0c29; color: white; padding: 20px; font-family: sans-serif;'>
                        <h2>Besoin d'un nouveau mot de passe ?</h2>
                        <p>Une demande de réinitialisation a été faite pour votre compte Astralis.</p>
                        <p>Cliquez sur le lien ci-dessous pour choisir un nouveau mot de passe :</p>
                        <a href='{resetLink}' style='color: #a29bfe; font-size: 18px;'>Réinitialiser mon mot de passe</a>
                        <p>Ce lien expire dans 1 heure. Si vous n'êtes pas à l'origine de cette demande, ignorez ce message.</p>
                    </div>";

                await _emailService.SendEmailAsync(user.Email, subject, message);
            }
            catch
            {
            }

            return Ok(new { message = "Si cet email existe, un lien a été envoyé." });
        }

        /// <summary>
        /// Resets the user's password using a valid token.
        /// </summary>
        [HttpPost("Reset-Password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == dto.Token);

            if (user == null || user.TokenExpiration < DateTime.UtcNow)
            {
                return BadRequest("Le lien est invalide ou a expiré.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.VerificationToken = null;
            user.TokenExpiration = null;
            user.IsEmailVerified = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Mot de passe modifié avec succès." });
        }

        // That method generates the JWT, creates the HttpOnly Cookie, and returns the DTO.
        private ActionResult<AuthResponseDto> GenerateSession(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRoleNavigation?.Label ?? "Membre"),
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
                SameSite = SameSiteMode.None,
                Expires = expiresAt
            };

            Response.Cookies.Append("authToken", jwt, cookieOptions);

            return Ok(new AuthResponseDto
            {
                Id = user.Id,
                Token = jwt,
                Expiration = expiresAt,
                Username = user.Username,
                Role = user.UserRoleNavigation?.Label ?? "Membre",
                AvatarUrl = user.AvatarUrl,
                IsPremium = user.IsPremium
            });
        }
    }
}