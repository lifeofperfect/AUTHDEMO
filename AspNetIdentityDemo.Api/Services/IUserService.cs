using AspNetIdentityDemo.Api.Models.Request.Dto;
using AspNetIdentityDemo.Api.Models.Response;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Api.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUser(RegisterViewModel request);
    }

    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserManagerResponse> RegisterUser(RegisterViewModel request)
        {
            if(request == null)
            {
                throw new NullReferenceException("Register Model is null");
            }

            if (request.Password != request.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Confirm password dosent match the password",
                    IsSuccess = false,
                };
            var identityRequest = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email,

            };

            var check = await _userManager.FindByNameAsync(request.Email);

            if(check != null)
            {
                return new UserManagerResponse
                {
                    Message = "User already exist",
                    IsSuccess = false,

                };
            }

            var response = await _userManager.CreateAsync(identityRequest, request.Password);

            if (response.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "User created successfully",
                    IsSuccess = true,

                };
            }
            return new UserManagerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = response.Errors.Select(e => e.Description)
            };
        }
    }
}
