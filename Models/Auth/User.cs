using Microsoft.AspNetCore.Identity;

namespace SimpleSprint.Models.Auth
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public string LastName { get; set; }

    }
}