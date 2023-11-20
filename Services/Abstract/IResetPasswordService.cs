using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Services.Abstract
{
    public interface IResetPasswordService
    {
        public Task<DataResult> GetResetPasswordOTP(GetResetPasswordOTPRequest otpPassword);
        public Task<DataResult> ResetPassword(ResetPasswordRequest resetPassword);
    }
}
