using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Services.Abstract
{
    public interface ITokenService
    {
        public Task<GenerateTokenResponse> GenerateToken(User user);
    }
}
