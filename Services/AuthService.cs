using Azure.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using UserAuthAPI.DataAccess;
using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.DataAccess.EntityFramework;
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
        private readonly IOTPRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenHelper _tokenHelper;
        public AuthService(
            IConfiguration configuration,
            IOTPRepository otpRepository, 
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,  
            ITokenHelper tokenHelper
        ) {
            _configuration = configuration;
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenHelper = tokenHelper;
        }

        public async Task<DataResult> GetLoginOTP(GetLoginOTPRequest request)
        {
            var msgList = new List<MessageItem>();

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
            _otpRepository.SaveChanges();

            return new DataResult { Success = true, Data = new GetLoginOTPResponse { OtpToken = msgOtpToken } };
        }

        public async Task<DataResult> Login(LoginUserRequest request)
        {
            var msgList = new List<MessageItem>();

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
                _otpRepository.SaveChanges();
            }
            else
            {
                otp.NumberOfAttempts = otp.NumberOfAttempts + 1;
                _otpRepository.Update(otp);
                _otpRepository.SaveChanges();
                msgList.Add(new MessageItem() { Message = "Girdiğiniz tek kullanımlık şifre hatalı!" });
                return new DataResult { Messages = msgList };
            }

            var user = _userRepository.Get(u => u.Id == otp.UserId && u.IsActive);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Hesap askıya alınmış!" });
                return new DataResult { Messages = msgList };
            }

            var accessToken = await _tokenHelper.GenerateAccessToken(user);
            var refreshToken = await _tokenHelper.GenerateRefreshToken(user);
            _refreshTokenRepository.Add(refreshToken);
            _refreshTokenRepository.SaveChanges();
            accessToken.RefreshToken = refreshToken.RefToken;
            msgList.Add(new MessageItem() { Message = "Başarıyla giriş yapıldı!" });
            return new DataResult { Messages= msgList, Success = true, Data = accessToken };
        }

        public async Task<DataResult> Register(RegisterUserRequest request)
        {
            var msgList = new List<MessageItem>();

            var user = _userRepository.Get(u => u.PhoneNumber == request.PhoneNumber);
            if (user != null)
            {
                msgList.Add(new MessageItem() { Message = "Sistemde bu telefon numarası ile zaten kayıtlı kullanıcı var!" });
                return new DataResult { Messages = msgList };
            }

            HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

            _userRepository.Add(new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Role = "User",
                IsActive = true,
                CreateDate = DateTime.UtcNow,
                PhoneVerified = false,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
            _userRepository.SaveChanges();

            user = _userRepository.Get(u => u.PhoneNumber == request.PhoneNumber && u.IsActive);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Yeni kullanıcı kaydı başarısız!" });
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
            _otpRepository.SaveChanges();

            return new DataResult { Success = true, Data = new GetLoginOTPResponse { OtpToken = msgOtpToken } };
        }

        public async Task<DataResult> RefreshToken(RefreshTokenRequest request)
        {
            var msgList = new List<MessageItem>();

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
                msgList.Add(new MessageItem() { Message = "Hesap askıya alınmış!" });
                return new DataResult { Messages = msgList };
            }

            /* # Remove Refresh Token # */
            getRef.IsValid = false;
            _refreshTokenRepository.Update(getRef);
            _refreshTokenRepository.SaveChanges();
            /* # Remove Refresh Token # */

            var accessToken = await _tokenHelper.GenerateAccessToken(user);
            var refreshToken = await _tokenHelper.GenerateRefreshToken(user);
            _refreshTokenRepository.Add(refreshToken);
            _refreshTokenRepository.SaveChanges();
            accessToken.RefreshToken = refreshToken.RefToken;

            msgList.Add(new MessageItem() { Message = "Oturum yenilendi." });
            return new DataResult { Messages = msgList, Success = true, Data = accessToken };
        }


    }
}
