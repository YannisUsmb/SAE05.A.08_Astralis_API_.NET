using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [DisplayName("User")]
    public class UsersController : CrudController<User, UserDetailDto, UserDetailDto, UserCreateDto, UserUpdateDto, int>
    {
        public UsersController(IUserRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        /// <summary>
        /// Retrieves all users (Admin only).
        /// </summary>
        /// <returns>A list of all registered users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized (requires Admin role).</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override Task<ActionResult<IEnumerable<UserDetailDto>>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Retrieves a specific user profile (Admin only).
        /// </summary>
        /// <remarks>Users can only view their own profile, unless they have the Admin role.</remarks>
        /// <param name="id">The user ID.</param>
        /// <returns>The requested user profile.</returns>
        /// <response code="200">Returns the user profile.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to view this profile.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<UserDetailDto>> GetById(int id)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            if (userRole != "Admin" && userId != id)
            {
                return Forbid();
            }

            return await base.GetById(id);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="createDto">The registration details.</param>
        /// <returns>The created user profile.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<UserDetailDto>> Post(UserCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User entity = _mapper.Map<User>(createDto);

            entity.UserRoleId = 2;
            entity.InscriptionDate = DateOnly.FromDateTime(DateTime.Now);
            entity.IsPremium = false;

            await _repository.AddAsync(entity);

            return Ok(_mapper.Map<UserDetailDto>(entity));
        }

        /// <summary>
        /// Updates a user profile (User only).
        /// </summary>
        /// <remarks>Users can only update their own profile.</remarks>
        /// <param name="id">The user ID.</param>
        /// <param name="updateDto">The updated profile details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Profile updated successfully.</response>
        /// <response code="400">Invalid input data or ID mismatch.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to update this profile.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<IActionResult> Put(int id, UserUpdateDto updateDto)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || id != userId)
            {
                return Forbid();
            }

            return await base.Put(id, updateDto);
        }

        /// <summary>
        /// Changes the authenticated user's password (User only).
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="passwordDto">The new password details.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Password changed successfully.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to change this password.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id}/Password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto passwordDto)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId) || id != userId)
            {
                return Forbid();
            }

            User? user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _mapper.Map(passwordDto, user);

            await _repository.UpdateAsync(user, user);

            return NoContent();
        }
    }
}