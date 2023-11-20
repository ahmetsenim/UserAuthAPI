using Microsoft.AspNetCore.Identity;
using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.DataAccess.EntityFramework;
using UserAuthAPI.Helpers;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Services.Abstract;
using static System.Net.WebRequestMethods;

namespace UserAuthAPI.Services
{
    public class ResetPasswordService: IResetPasswordService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordOTPRepository _resetPasswordOTPRepository;
        public ResetPasswordService(IConfiguration configuration, IResetPasswordOTPRepository resetPasswordOTPRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _resetPasswordOTPRepository = resetPasswordOTPRepository;
            _userRepository = userRepository;
        }

        public async Task<DataResult> GetResetPasswordOTP(GetResetPasswordOTPRequest request)
        {
            var msgList = new List<MessageItem>();

            var user = _userRepository.Get(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Bu telefon numarasına ait hesap bulunamadı!" });
                return new DataResult { Messages = msgList };
            }
            if (!user.IsActive)
            {
                msgList.Add(new MessageItem() { Message = "Hesap askıya alınmış!" });
                return new DataResult { Messages = msgList };
            }

            string tempPassword = EncryptedDataGenerator.RandomGenerate(8);
            ResetPasswordOTP tPwd = new ResetPasswordOTP()
            {
                UserId = user.Id,
                OTP = tempPassword,
                NumberOfAttempts = 0,
                ValidityDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["AppSettings:TempPasswordValidityPeriodMinutes"])),
                PasswordReset = false
            };
            _resetPasswordOTPRepository.Add(tPwd);
            _resetPasswordOTPRepository.SaveChanges();
            return new DataResult { Success = true, Data = request, Messages = msgList };
        }

        public async Task<DataResult> ResetPassword(ResetPasswordRequest request)
        {
            var msgList = new List<MessageItem>();

            var user = _userRepository.Get(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
            {
                msgList.Add(new MessageItem() { Message = "Bu telefon numarasına ait hesap bulunamadı!" });
                return new DataResult { Messages = msgList };
            }
            if (!user.IsActive)
            {
                msgList.Add(new MessageItem() { Message = "Hesap askıya alınmış!" });
                return new DataResult { Messages = msgList };
            }

            var pw = _resetPasswordOTPRepository.Find(x => x.UserId == user.Id).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
            if (pw == null)
            {
                msgList.Add(new MessageItem() { Message = "Şifre sıfırlama kodu hatalı!" });
                return new DataResult { Messages = msgList };
            }

            if (pw.ValidityDate <= DateTime.UtcNow)
            {
                msgList.Add(new MessageItem() { Message = "Şifre sıfırlama kodunun süresi doldu! Lütfen yeni şifre sıfırlama talebi oluşturun." });
                return new DataResult { Messages = msgList };
            }

            if (pw.NumberOfAttempts >= 3)
            {
                msgList.Add(new MessageItem() { Message = "Şifre sıfırlama kodunu çok sayıda hatalı girdiniz! Lütfen daha sonra tekrar deneyin." });
                return new DataResult { Messages = msgList };
            }

            HashingHelper.CreatePasswordHash(request.NewPassword, out var passwordSalt, out var passwordHash);
            if (pw.OTP == request.OTP)
            {
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _userRepository.Update(user);
                _userRepository.SaveChanges();

                // todo şifreyi sıfırla
                pw.NumberOfAttempts = pw.NumberOfAttempts + 1;
                pw.PasswordReset = true;
                _resetPasswordOTPRepository.Update(pw);
                _resetPasswordOTPRepository.SaveChanges();
            }
            else
            {
                pw.NumberOfAttempts = pw.NumberOfAttempts + 1;
                _resetPasswordOTPRepository.Update(pw);
                _resetPasswordOTPRepository.SaveChanges();
                msgList.Add(new MessageItem() { Message = "Şifre sıfırlama kodu hatalı!" });
                return new DataResult { Messages = msgList };
                //otp.NumberOfAttempts = otp.NumberOfAttempts + 1;
                //_otpRepository.Update(otp);
                //_otpRepository.SaveChanges();
                //msgList.Add(new MessageItem() { Message = "Girdiğiniz tek kullanımlık şifre hatalı!" });
                //return new DataResult { Messages = msgList };
            }

            string resetOTP = EncryptedDataGenerator.RandomGenerate(8);
            ResetPasswordOTP tPwd = new ResetPasswordOTP()
            {
                UserId = user.Id,
                OTP = resetOTP,
                NumberOfAttempts = 0,
                ValidityDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["AppSettings:ResetPasswordValidityPeriodMinutes"])),
                PasswordReset = false
            };
            _resetPasswordOTPRepository.Add(tPwd);
            _resetPasswordOTPRepository.SaveChanges();
            return new DataResult { Success = true, Data = request, Messages = msgList };
        }
    }
}
