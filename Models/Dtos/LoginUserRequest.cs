namespace UserAuthAPI.Models.Dtos
{
    public class LoginUserRequest
    {
        public string OtpToken { get; set; }
        public int OtpCode { get; set; }
    }
}
