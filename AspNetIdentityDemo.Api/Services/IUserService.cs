using AspNetIdentityDemo.Api.Models.Request.Dto;
using AspNetIdentityDemo.Api.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Api.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUser(RegisterViewModel request);
        Task<UserManagerResponse> Login(LoginViewModel request);
    }

    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _config;
        public UserService(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;

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

        public async Task<UserManagerResponse> Login(LoginViewModel request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);

            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no user with that email address",
                    IsSuccess = false
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false
                };
            }

            var claims = new[]
            {
                new Claim("Email", request.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["AuthSettings:Issuer"],
                audience: _config["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }
    }
}
