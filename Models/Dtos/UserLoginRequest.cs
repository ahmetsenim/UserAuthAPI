namespace UserAuthAPI.Models.Dtos
{
    public class UserLoginRequest
    {
        public string OtpToken { get; set; }
        public int OtpCode { get; set; }
    }
}
