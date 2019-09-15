using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleSprint.Business.Interfaces;
using SimpleSprint.Contracts.Auth.Requests;
using SimpleSprint.Contracts.Auth.Responses;
using SimpleSprint.Dtos;
using SimpleSprint.Models.Auth;
using SimpleSprint.Repos.Interfaces;

namespace SimpleSprint.Business {
    public class AuthBusiness : IAuthBusiness {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        public AuthBusiness (IAuthRepository authRepo,
            IConfiguration config,
            SignInManager<User> signInManager,
            UserManager<User> userManager
        ) {
            _authRepo = authRepo;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<bool> CreateUser (CreateUserRequest request) {

            var isEmailExists = await _authRepo.IsUserEMailExists (request.Email);

            var isUserNameExists = await _authRepo.IsUserNameExists (request.UserName);

            if (isUserNameExists) {
                throw new Exception ("UserName is already taken");
            }

            if (isEmailExists) {
                throw new Exception ("Email is already saved in system");
            }

            var result = await _userManager.CreateAsync (new User () {
                Email = request.Email,
                LastName = request.LastName,
                Name = request.FirstName,
                UserName = request.UserName
            }, request.Password);

            if (result.Errors.Count () > 0) {
                throw new Exception (result.Errors.FirstOrDefault ().Description);
            }

            return result.Succeeded;
       
        }

        public async Task<LoginResponse> Login (LoginRequest request) {
            var user = await _authRepo.GetUser (request.UserName);

            if (user == null) {
                return null;
            }

            var result = await _signInManager
                .CheckPasswordSignInAsync (user, request.Password, false);

    

            if (!result.Succeeded) {
                return null;
            }

            var claims = new [] {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config.GetSection ("Application:Secret").Value));

            var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor () {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddHours (6),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler ();

            var token = tokenHandler.CreateToken (tokenDescriptor);

            return new LoginResponse () {
                Token = tokenHandler.WriteToken (token),
                    User = new Contracts.Auth.DtoContracts.User () {
                        Email = user.Email,
                        LastName = user.LastName,
                        Name = user.Name,
                        UserName = user.UserName
                        }
            };
        }

        
    }
}