using SimpleSprint.Contracts.Auth.DtoContracts;

namespace SimpleSprint.Contracts.Auth.Responses
{
    public class LoginResponse : ResponseBase
    {
        public User User { get; set; }
        public string Token { get; set; }
    }
}