using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Helpers;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Services.Abstract;

namespace UserAuthAPI.Services
{
    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateRefreshToken(User user)
        {
            string refToken = EncryptedDataGenerator.RandomGenerate(16) + "-" + EncryptedDataGenerator.RandomGenerate(16) + "-" + EncryptedDataGenerator.RandomGenerate(16);
            RefreshToken rToken = new RefreshToken()
            {
                UserId = user.Id,
                RefToken = refToken,
                CreateDate = DateTime.UtcNow,
                ValidityDate = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["AppSettings:RefreshTokenValidityPeriodDays"])),
                IsValid = true
            };
            _refreshTokenRepository.Add(rToken);
            _refreshTokenRepository.SaveChanges();
            return await Task.FromResult(refToken);
        }

        public async Task RemoveRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            _refreshTokenRepository.Update(refreshToken);
            _refreshTokenRepository.SaveChanges();
        }
    }
}
