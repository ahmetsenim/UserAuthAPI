using Azure.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using UserAuthAPI.DataAccess;
using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Helpers;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;
using static System.Net.WebRequestMethods;

namespace UserAuthAPI.Services
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IOTPRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(
            IConfiguration configuration,
            IAccessTokenService accessTokenService,
            IRefreshTokenService refreshTokenService,
            IOTPRepository otpRepository, 
            IUserRepository userRepository, 
            IRefreshTokenRepository refreshTokenRepository
        ) {
            _configuration = configuration;
            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<DataResult> GetOTPAsync(GetOTPRequest request)
        {
            List<MessageItem> msgList = new();

            var user = _userRepository.Get(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null) {
                msgList.Add(new MessageItem() { Message= "Bu telefon numarasına ait hesap bulunamadı!" });
                return new DataResult { Messages = msgList }; 
            }
            if (!user.IsActive) {
                msgList.Add(new MessageItem() { Message = "Hesabınız askıya alınmış!" });
                return new DataResult { Messages = msgList }; 
            }
            if (!HashingHelper.VerifyPasswordHash(request.Password, user.PasswordSalt, user.PasswordHash)) {
                msgList.Add(new MessageItem() { Message = "Girdiğiniz bilgilere ait hesap bulunamadı!" });
                return new DataResult { Messages = msgList }; 
            }
            var userWaitOtp = _otpRepository.Get(u => u.UserId == user.Id && u.CreateDate > DateTime.UtcNow.AddSeconds(-60));
            if (userWaitOtp != null)
            {
                msgList.Add(new MessageItem() { Message = "Yeni doğrulama mesajı almak için bir süre beklemelisiniz!" });
                return new DataResult { Messages = msgList };
            }
            int msgOtp = OtpGenerator.CreateOtp();
            string msgOtpToken = OtpGenerator.CreateOtpToken();

            _otpRepository.Add(new OTP
            {
                OtpCode = msgOtp,
                OtpToken = msgOtpToken,
                UserId = user.Id,
                IsLoggedIn = false,
                NumberOfAttempts = 0,
                CreateDate = DateTime.UtcNow,
                ValidityDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["AppSettings:OTPValidityPeriodMinutes"])),
                LoginDate = DateTime.UtcNow,
            });
            await _otpRepository.SaveChangesAsync();
            return new DataResult { Success = true, Data = new GetOTPResponse { OtpToken = msgOtpToken } };
        }

        public async Task<DataResult> LoginUserAsync(UserLoginRequest request)
        {
            List<MessageItem> msgList = new();

            var otp = _otpRepository.Get(u => u.OtpToken == request.OtpToken && u.IsLoggedIn == false);

            if (otp == null)
            {
                msgList.Add(new MessageItem() { Message = "Girdiğiniz tek kullanımlık şifre hatalı!" });
                return new DataResult { Messages = msgList };
            }

            if (otp.ValidityDate <= DateTime.UtcNow)
            {
                msgList.Add(new MessageItem() { Message = "Tek kullanımlık şifrenizin süresi doldu! Lütfen yeniden giriş yapmayı deneyiniz. " });
                return new DataResult { Messages = msgList };
            }

            if (otp.NumberOfAttempts >= 3)
            {
                msgList.Add(new MessageItem() { Message = "Tek kullanımlık şifreyi çok sayıda hatalı girdiniz! Lütfen yeniden giriş yapmayı deneyiniz. " });
                return new DataResult { Messages = msgList };
            }

            if (otp.OtpCode == request.OtpCode)
            {
                otp.NumberOfAttempts = otp.NumberOfAttempts + 1;
                otp.LoginDate = DateTime.UtcNow;
                otp.IsLoggedIn = true;
                _otpRepository.Update(otp);
                await _otpRepository.SaveChangesAsync();
            }
            else
            {
                otp.NumberOfAttempts = otp.NumberOfAttempts + 1;
                _otpRepository.Update(otp);
                await _otpRepository.SaveChangesAsync();
                msgList.Add(new MessageItem() { Message = "Girdiğiniz tek kullanımlık şifre hatalı!" });
                return new DataResult { Messages = msgList };
            }

            var user = _userRepository.Get(u => u.Id == otp.UserId && u.IsActive);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Hesap bulunamadı!" });
                return new DataResult { Messages = msgList };
            }

            var accessToken = await _accessTokenService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user);
            accessToken.RefreshToken = refreshToken;
            msgList.Add(new MessageItem() { Message = "Başarıyla giriş yapıldı!" });
            return new DataResult { Messages= msgList, Success = true, Data = accessToken };
        }

        public async Task<DataResult> LoginUserWithRefreshTokenAsync(RefreshTokenRequest request)
        {
            List<MessageItem> msgList = new();

            var getRef = _refreshTokenRepository.Get(u => u.UserId == request.UserId && u.RefToken == request.RefToken && request.RefToken != "");
            if (getRef == null)
            {
                msgList.Add(new MessageItem() { Message = "Oturum süresi doldu! Lütfen tekrar giriş yapınız." });
                return new DataResult { Messages = msgList };
            }
            if (!getRef.IsValid)
            {
                msgList.Add(new MessageItem() { Message = "Oturum süresi doldu! Lütfen tekrar giriş yapınız." });
                return new DataResult { Messages = msgList };
            }

            if (getRef.ValidityDate <= DateTime.UtcNow)
            {
                msgList.Add(new MessageItem() { Message = "Oturum süresi doldu! Lütfen tekrar giriş yapınız." });
                return new DataResult { Messages = msgList };
            }

            var user = _userRepository.Get(u => u.Id == request.UserId && u.IsActive);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Hesap bulunamadı!" });
                return new DataResult { Messages = msgList };
            }

            await _refreshTokenService.RemoveRefreshToken(getRef);
            var accessToken = await _accessTokenService.GenerateToken(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user);
            accessToken.RefreshToken = refreshToken;
            return new DataResult { Success = true, Data = accessToken };
        }
    }
}
