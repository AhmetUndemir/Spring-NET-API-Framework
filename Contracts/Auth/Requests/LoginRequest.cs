using System.ComponentModel.DataAnnotations;

namespace SimpleSprint.Contracts.Auth.Requests
{
    public class LoginRequest : RequestBase
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}