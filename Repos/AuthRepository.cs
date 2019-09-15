using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleSprint.Data.Interfaces;
using SimpleSprint.Dtos;
using SimpleSprint.Models.Auth;
using SimpleSprint.Repos.Interfaces;

namespace SimpleSprint.Repos {
    public class AuthRepository : IAuthRepository {
        private readonly IUnitOfWork _uow;
        public AuthRepository (IUnitOfWork uow) {
            _uow = uow;
        }

        ~AuthRepository () {
            _uow.Dispose ();
        }
        public async Task<int> CreateUser (UserDto user) {
            var newUser = new User () {
                Email = user.Email,
                UserName = user.UserName,
                Name = user.Name,
                LastName = user.LastName,
                PasswordHash = user.Password
            };

            _uow.User.Insert (newUser);
            await _uow.SaveChangesAsync ();

            return newUser.Id;
        }

        public async Task<bool> IsUserEMailExists (string email) {
            var user = await _uow.User.Table.FirstOrDefaultAsync (x => x.Email.Equals (email));

            if (user == null)
                return false;

            return true;
        }

        public async Task<bool> IsUserNameExists (string userName) {
            var user = await _uow.User.Table.FirstOrDefaultAsync (x => x.UserName.Equals (userName));

            if (user == null)
                return false;

            return true;
        }

        public async Task<User> GetUser (string username) {
            var user = await _uow.User.Table
                .FirstOrDefaultAsync (x => x.UserName.ToLower ().Equals (username));

            return user;
        }
    }

}