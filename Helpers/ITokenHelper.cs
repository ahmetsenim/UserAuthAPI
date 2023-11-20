using Azure.Core;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Helpers
{
    public interface ITokenHelper
    {
        public Task<AccessTokenResponse> GenerateAccessToken(User user);
        public Task<RefreshToken> GenerateRefreshToken(User user);
        public bool ValidateToken(string token);
    }
}
