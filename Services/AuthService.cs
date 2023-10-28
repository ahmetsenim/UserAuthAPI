using Azure.Core;
using UserAuthAPI.DataAccess;
using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Helpers;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserAuthAPI.Services
{
    public class AuthService: IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IOTPRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(
            ITokenService tokenService, 
            IOTPRepository otpRepository, 
            IUserRepository userRepository, 
            IRefreshTokenRepository refreshTokenRepository
        ) {
            _tokenService = tokenService;
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

            // Todo : Send otp sms
            // SmsServiceManager ssm = new SmsServiceManager();
            // ssm.Send("", "", "");

            _otpRepository.Add(new OTP
            {
                OtpCode = msgOtp,
                OtpToken = msgOtpToken,
                UserId = user.Id,
                IsLoggedIn = false,
                NumberOfAttempts = 0,
                CreateDate = DateTime.UtcNow,
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

            if (otp.CreateDate <= DateTime.UtcNow.AddMinutes(-5))
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

            var accessToken = await _tokenService.GenerateToken(user);

            var claims = _userRepository.GetClaims(user.Id);
            accessToken.Claims = claims.Select(x => x.Name).ToList();

            //if (user.IsSeller)
            //{
            //    var sellerMenu = SellerMenuCreator.GetSellerMenu(claims);
            //    _cacheManager.Add($"{CacheKeys.UserIdForSellerMenu}={user.SellerId}={user.UserId}", sellerMenu);
            //}

            string refToken = EncryptedDataGenerator.RandomGenerate(16) + "-" + EncryptedDataGenerator.RandomGenerate(16) + "-" + EncryptedDataGenerator.RandomGenerate(16);
            RefreshToken rToken = new RefreshToken()
            {
                UserId = user.Id,
                RefToken = refToken,
                CreateDate = DateTime.UtcNow,
                IsValid = true
            };
            _refreshTokenRepository.Add(rToken);
            await _refreshTokenRepository.SaveChangesAsync();
            accessToken.RefreshToken = refToken;

            //_cacheManager.Remove($"{CacheKeys.UserIdForClaim}={user.UserId}");
            //_cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", claims.Select(x => x.Name));

            //if (user.IsSeller)
            //{
            //    var sellerMenu = SellerMenuGenerator.CreateSellerMenu(claims);
            //    _cacheManager.Remove($"{CacheKeys.UserIdForSellerMenu}={user.UserId}");
            //    _cacheManager.Add($"{CacheKeys.UserIdForSellerMenu}={user.UserId}", sellerMenu);
            //}


            //List<SellerMenu> sm = new List<SellerMenu>();
            //sm

            //return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
            return new DataResult { Success = true, Data = accessToken };
        }


        //{
        //    UserLoginResponse response = new();

        //    if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        //    {
        //        throw new ArgumentNullException(nameof(request));
        //    }

        //    if (request.UserName == "onur" && request.Password == "123456")
        //    {
        //        var generatedTokenInformation = await _tokenService.GenerateToken(new GenerateTokenRequest { UserName = request.UserName, UserID = "111" });

        //        response.Status = true;
        //        response.Token = generatedTokenInformation.Token;
        //    }
        //    else
        //    {
        //        response.Message = "Kullanıcı bulunamadı!";
        //    }

        //    return response;
        //}

    }
}
