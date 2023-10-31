using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Services.Abstract
{
    public interface IAuthService
    {
        public Task<DataResult> GetOTPAsync(GetOTPRequest request);
        public Task<DataResult> LoginUserAsync(UserLoginRequest request);
        public Task<DataResult> LoginUserWithRefreshTokenAsync(RefreshTokenRequest request);

    }
}
