using System.Threading.Tasks;
using SimpleSprint.Dtos;
using SimpleSprint.Models.Auth;

namespace SimpleSprint.Repos.Interfaces
{
    public interface IAuthRepository
    {
         Task<int> CreateUser(UserDto user);

         Task<bool> IsUserNameExists(string userName);

         Task<bool> IsUserEMailExists(string email);

         Task<User> GetUser(string username);
         
    }
}