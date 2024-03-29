using System.ComponentModel.DataAnnotations;

namespace SimpleSprint.Contracts.Auth.Requests {
    public class CreateUserRequest : RequestBase {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength (20, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 20 chracters")]
        public string Password { get; set; }

    }
}