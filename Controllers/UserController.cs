using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Runtime.CompilerServices;
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
        readonly IResetPasswordService resetPasswordService;

        public UserController(IAuthService authService, IResetPasswordService resetPasswordService)
        {
            this.authService = authService;
            this.resetPasswordService = resetPasswordService;
        }

        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("GetLoginOTP")]
        public async Task<ActionResult<GetLoginOTPResponse>> GetLoginOTP([FromBody] GetLoginOTPRequest request)
        {
            var result = await authService.GetLoginOTP(request);
            return result.Success ? Ok(result.Data) : Unauthorized(result.Messages);
        }

        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request)
        {
            var result = await authService.Login(request);
            return result.Success ? Ok(result.Data) : Unauthorized(result.Messages);
        }

        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("Register")]
        public async Task<ActionResult<GetLoginOTPResponse>> Register([FromBody] RegisterUserRequest request)
        {
            var result = await authService.Register(request);
            return result.Success ? Ok(result.Data) : BadRequest(result.Messages);
        }


        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<LoginUserResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RefreshToken(request);
            return result.Success ? Ok(result.Data) : BadRequest(result.Messages);
        }


        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("GetResetPasswordOTP")]
        public async Task<ActionResult<GetResetPasswordOTPResponse>> GetResetPasswordOTP([FromBody] GetResetPasswordOTPRequest request)
        {
            var result = await resetPasswordService.GetResetPasswordOTP(request);
            return result.Success ? Ok(result.Data) : BadRequest(result.Messages);
        }


        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<LoginUserResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await resetPasswordService.ResetPassword(request);
            return result.Success ? Ok(result.Data) : BadRequest(result.Messages);
        }


        //[HttpPost("Test")]
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Admin, Ogrenci")]
        //public async Task<ActionResult<string>> TestAsync([FromBody] string request)
        //{
        //    return request == "" ? Ok("Başarılı") : BadRequest("Hatalı Veri!");
        //}

    }
}
