using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User? GetUser(int id);
        bool IsUniqueUser(string username);
        bool Login(UserLoginDto userLoginDto);
        User Register(CreateUserDto createUserDto);
    }
}
