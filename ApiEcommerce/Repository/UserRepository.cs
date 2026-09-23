using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserById(string id)
        {
            return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            return !(await _context.ApplicationUsers.AnyAsync(u => u.UserName == userName));
        }

        public async Task<ApplicationUser?> Login(ApplicationUserLoginDto userLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user is null) { 
                return null; 
            
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, lockoutOnFailure: true);
            return result.Succeeded ? user : null;
        }

        public async Task<ApplicationUser?> Register(CreateApplicationUserDto createUserDto)
        {
            var user = new ApplicationUser
            {
                Name = createUserDto.Name,
                UserName = createUserDto.UserName,
                Email = createUserDto.Email
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            return result.Succeeded ? user : null;
        }
    }
}
