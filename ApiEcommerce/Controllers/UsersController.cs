using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers() {
            var users = _repository.GetUsers();
            var usersDto = _mapper.Map<UserDto>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUserById(int id) {
            if (id <= 0)
            {
                return BadRequest();
            }

            var user = _repository.GetUserById(id);
            if (user is null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponseDto>> Login(UserLoginDto userLoginDto) {
            if (userLoginDto is null)
            {
                return BadRequest();
            }

            var user = _repository.Login(userLoginDto);
            if (user is null)
            {
                return LoginFail();
            }

            return await BuildToken(user);
        }

        [HttpPost("register", Name = "Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponseDto>> Register(CreateUserDto createUserDto) {
            if (createUserDto is null)
            {
                return BadRequest();
            }

            var isUniqueUser = _repository.IsUniqueUser(createUserDto.Username);
            if (!isUniqueUser)
            {
                ModelState.AddModelError(string.Empty, "El usuario ya existe");
                return ValidationProblem();
            }

            var user = _repository.Register(createUserDto);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Error al registrar el usuario");
                return StatusCode(500, ModelState);
            }

            return await BuildToken(user);
        }

        private async Task<LoginResponseDto> BuildToken(
            User user)
        {
            var claims = new List<Claim>
            {
                new Claim("username", user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            var securityToken = new JwtSecurityToken(issuer: _configuration["JwtSettings:Issuer"]!, audience: _configuration["JwtSettings:Audience"]!,
                claims: claims, expires: expiration, signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration
            };
        }

        private ActionResult LoginFail()
        {
            ModelState.AddModelError(string.Empty, "Login incorrecto");
            return ValidationProblem();
        }
    }
}
