using System.Threading.Tasks;
using SimpleSprint.Contracts.Auth.Requests;
using SimpleSprint.Contracts.Auth.Responses;
using SimpleSprint.Dtos;

namespace SimpleSprint.Business.Interfaces
{
    public interface IAuthBusiness 
    {
        Task<bool> CreateUser(CreateUserRequest request);
        Task<LoginResponse> Login(LoginRequest request);
    }
}