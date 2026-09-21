using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> GetUsers();
        ApplicationUser? GetUserById(string id);
        bool IsUniqueUser(string userName);
        Task<ApplicationUser?> Login(ApplicationUserLoginDto userLoginDto);
        Task<ApplicationUser?> Register(CreateApplicationUserDto createUserDto);
    }
}
