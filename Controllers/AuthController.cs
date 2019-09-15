using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSprint.Business.Interfaces;
using SimpleSprint.Contracts.Auth.Requests;
using SimpleSprint.Contracts.Auth.Responses;
using SimpleSprint.Dtos;

namespace SimpleSprint.Controllers {
    [Route ("api/[controller]")]
    public class AuthController : BaseController {
        private readonly IAuthBusiness _authBusiness;
        public AuthController (IAuthBusiness authBusiness) {
            _authBusiness = authBusiness;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route ("CreateUser")]
        public async Task<CreateUserResponse> CreateUser ([FromBody] CreateUserRequest request) {
            var IsSuccess = await _authBusiness.CreateUser (request);

            return new CreateUserResponse () {
                IsSuccess = IsSuccess
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [Route ("Login")]
        public async Task<LoginResponse> Login (LoginRequest request) {
            LoginResponse response = await _authBusiness.Login (request);

            if (response == null) {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            return response;
        }

        [HttpPost]
        [Route ("TestMethod")]
        public bool TestMethod () {
            return true;
        }

    }
}