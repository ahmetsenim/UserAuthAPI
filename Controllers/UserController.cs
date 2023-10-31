using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;

namespace UserAuthAPI.Controllers
{
    [Authorize]
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
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("GetOTP")]
        public async Task<ActionResult<GetOTPResponse>> GetOTPAsync([FromBody] GetOTPRequest request)
        {
            var result = await authService.GetOTPAsync(request);
            return result.Success ? Ok(result.Data) : Unauthorized(result.Messages);
        }

        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginResponse>> LoginUserAsync([FromBody] UserLoginRequest request)
        {
            var result = await authService.LoginUserAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result.Messages);
        }


        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UserLoginResponse>> LoginUserWithRefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.LoginUserWithRefreshTokenAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }




        [HttpPost("Test")]
        [Authorize(Roles = "Futbolcu")]
        public async Task<ActionResult<string>> TestAsync([FromBody] string request)
        {
            return request == "" ? Ok("Başarılı") : BadRequest("Hatalı Veri!");
        }

        [HttpPost("Test2")]
        [Authorize(Roles = "Futbolcu, Antrenor")]
        public async Task<ActionResult<string>> Test2Async([FromBody] string request)
        {
            return request == "" ? Ok("Başarılı") : BadRequest("Hatalı Veri!");
        }

        [HttpPost("Test3")]
        public async Task<ActionResult<string>> Test3Async([FromBody] string request)
        {
            return request == "" ? Ok("Başarılı") : BadRequest("Hatalı Veri!");
        }

    }
}
