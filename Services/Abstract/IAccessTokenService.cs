using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Services.Abstract
{
    public interface IAccessTokenService
    {
        public Task<AccessTokenResponse> GenerateToken(User user);
    }
}
