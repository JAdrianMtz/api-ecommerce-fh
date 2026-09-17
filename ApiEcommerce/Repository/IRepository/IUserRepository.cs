using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User? GetUserById(int id);
        bool IsUniqueUser(string username);
        User? Login(UserLoginDto userLoginDto);
        User Register(CreateUserDto createUserDto);
    }
}
