using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using BCrypt.Net;

namespace ApiEcommerce.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public bool IsUniqueUser(string username)
        {
            return !_context.Users.Any(u => u.Username == username);
        }

        public User? Login(UserLoginDto userLoginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userLoginDto.Username);
            if (user is null) { 
                return null; 
            
            }
            var verifyPassword = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);
            return verifyPassword ? user : null;
        }

        public User Register(CreateUserDto createUserDto)
        {
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            var user = new User
            {
                Name = createUserDto.Name,
                Username = createUserDto.Username,
                Password = encryptedPassword,
                Role = createUserDto.Role
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
    }
}
