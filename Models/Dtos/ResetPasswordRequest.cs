namespace UserAuthAPI.Models.Dtos
{
    public class ResetPasswordRequest: GetResetPasswordOTPRequest
    {
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
