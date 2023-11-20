using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Services.Abstract
{
    public interface IAuthService
    {
        public Task<DataResult> GetLoginOTP(GetLoginOTPRequest request);
        public Task<DataResult> Login(LoginUserRequest request);
        public Task<DataResult> Register(RegisterUserRequest request);
        public Task<DataResult> RefreshToken(RefreshTokenRequest request);


        // public Task<DataResult> RefreshToken(RefreshTokenRequest request);
    }
}
