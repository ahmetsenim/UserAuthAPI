using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;

namespace UserAuthAPI.Services
{
    public class AccessTokenService:IAccessTokenService
    {
        private readonly IConfiguration _configuration;
        public AccessTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AccessTokenResponse> GenerateToken(User user)
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
    }
}
