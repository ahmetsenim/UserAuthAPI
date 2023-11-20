using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;
        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AccessTokenResponse> GenerateAccessToken(User user)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["AppSettings:Secret"]));
            int validityMinutes = Convert.ToInt32(_configuration["AppSettings:AccessTokenValidityPeriodMinutes"]);
            var dateTimeNow = DateTime.UtcNow;
            var dateTimeExpires = dateTimeNow.Add(TimeSpan.FromMinutes(validityMinutes));
            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: _configuration["AppSettings:ValidIssuer"],
                    audience: _configuration["AppSettings:ValidAudience"],
                    claims: new List<Claim> {
                    new Claim("UserID", user.Id.ToString()),
                    new Claim("PhoneNumber", user.PhoneNumber),
                    new Claim(ClaimTypes.Role, user.Role),
                    },
                    notBefore: dateTimeNow,
                    expires: dateTimeExpires,
                    signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );

            return await Task.FromResult(new AccessTokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                Expiration = dateTimeExpires
            });
        }

        public async Task<RefreshToken> GenerateRefreshToken(User user)
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
            //_refreshTokenRepository.Add(rToken);
            //_refreshTokenRepository.SaveChanges();
            return await Task.FromResult(rToken);
        }

        public bool ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
