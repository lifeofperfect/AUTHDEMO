using AspNetIdentityDemo.Api.Models.Request.Dto;
using AspNetIdentityDemo.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        //api/auth/register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel request)
        {
            if (ModelState.IsValid)
            {
                

                var result = await _userService.RegisterUser(request);

                if (result.IsSuccess)
                {
                    return Ok(result);

                }
                else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest("Some properties are not valid");
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel request)
        {
            if (ModelState.IsValid)
            {


                var response = await _userService.Login(request);

                if (response.IsSuccess)
                {
                    return Ok(response);

                }
                else
                {
                    return BadRequest(response);
                }
            }
            return BadRequest("Some properties are not valid");
        }
    }
}
