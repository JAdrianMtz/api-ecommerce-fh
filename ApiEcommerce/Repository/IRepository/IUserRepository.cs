using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetUsers();
        Task<ApplicationUser?> GetUserById(string id);
        Task<bool> IsUniqueUser(string userName);
        Task<ApplicationUser?> Login(ApplicationUserLoginDto userLoginDto);
        Task<ApplicationUser?> Register(CreateApplicationUserDto createUserDto);
    }
}
