using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.Services.Abstract
{
    public interface IRefreshTokenService
    {
        public Task<string> GenerateRefreshToken(User user);
        public Task RemoveRefreshToken(RefreshToken refreshToken);
    }
}
