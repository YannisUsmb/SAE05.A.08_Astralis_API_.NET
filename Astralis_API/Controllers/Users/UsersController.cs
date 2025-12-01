using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Astralis_API.Models.EntityFramework;
using Astralis.Shared.DTOs;
using Astralis_API.Models.Repository;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // On garde le contexte ici uniquement pour la configuration du Mapper spécifique que vous avez faite
        private readonly AstralisDbContext _context;
        private readonly IUserRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository usersRepo)
        {
            _context = new AstralisDbContext();

            _usersRepository = usersRepo;

            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile(_context));
            });
            _mapper = configuration.CreateMapper();
        }

        [HttpGet]
        [ActionName("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
        {
            IEnumerable<User> users = await _usersRepository.GetAllAsync();

            if (users == null)
            {
                return NotFound();
            }

            IEnumerable<UserDetailDto> userDetailDtos = _mapper.Map<IEnumerable<UserDetailDto>>(users);
            return Ok(userDetailDtos);
        }

        [HttpGet("{id}")]
        [ActionName("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDetailDto>> GetUser(int id)
        {
            User user = await _usersRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            UserDetailDto userDetailDto = _mapper.Map<UserDetailDto>(user);
            return Ok(userDetailDto);
        }

        [HttpGet("{name}")]
        [ActionName("GetByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUserByName(string name)
        {
            IEnumerable<User> users = await _usersRepository.GetByKeyAsync(name);

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            IEnumerable<UserDetailDto> userDetailDtos = _mapper.Map<IEnumerable<UserDetailDto>>(users);
            return Ok(userDetailDtos);
        }

        [HttpGet("{id}")]
        [ActionName("GetByRoleId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetByRoleId(int id)
        {
            IEnumerable<User> users = await _usersRepository.GetByUserRoleIdAsync(id);

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            IEnumerable<UserDetailDto> userDetailDtos = _mapper.Map<IEnumerable<UserDetailDto>>(users);
            return Ok(userDetailDtos);
        }

        [HttpPut("{id}")]
        [ActionName("Put")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUser(int id, UserUpdateDto userUpdateDto)
        {
            if (id != userUpdateDto.Id)
            {
                return BadRequest("L'ID de l'URL ne correspond pas à l'ID du corps de la requête.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User userToUpdate = await _usersRepository.GetByIdAsync(id);

            if (userToUpdate == null)
            {
                return NotFound();
            }

            User userModified = _mapper.Map(userUpdateDto, userToUpdate);

            await _usersRepository.UpdateAsync(userToUpdate, userModified);

            return NoContent();
        }

        [HttpPost]
        [ActionName("Post")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDetailDto>> PostUser(UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = _mapper.Map<User>(userCreateDto);

            await _usersRepository.AddAsync(user);

            UserDetailDto userDetailDto = _mapper.Map<UserDetailDto>(user);

            return CreatedAtAction("GetById", new { id = user.Id }, userDetailDto);
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User user = await _usersRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _usersRepository.DeleteAsync(user);

            return NoContent();
        }
    }
}