using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;

namespace UserAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IAuthService authService;

        public UserController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("GetOTP")]
        public async Task<ActionResult<GetOTPResponse>> GetOTPAsync([FromBody] GetOTPRequest request)
        {
            var result = await authService.GetOTPAsync(request);
            return result.Success ? Ok(result.Data) : Unauthorized(result.Messages);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginResponse>> LoginUserAsync([FromBody] UserLoginRequest request)
        {
            var result = await authService.LoginUserAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result.Messages);
        }

    }
}
