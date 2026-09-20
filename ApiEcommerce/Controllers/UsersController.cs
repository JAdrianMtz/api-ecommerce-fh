using ApiEcommerce.Configurations;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UsersController(IUserRepository repository, IMapper mapper, IOptions<JwtSettings> jwtSettingsOptions)
        {
            _repository = repository;
            _mapper = mapper;
            _jwtSettings = jwtSettingsOptions.Value;
        }

        [HttpGet(Name = "GetUsers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<User>> GetUsers() {
            var users = _repository.GetUsers();
            var usersDto = _mapper.Map<UserDto>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                ModelState.AddModelError(nameof(createUserDto.Username), "El usuario ya existe");
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
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var securityToken = new JwtSecurityToken(issuer: _jwtSettings.Issuer, audience: _jwtSettings.Audience,
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
