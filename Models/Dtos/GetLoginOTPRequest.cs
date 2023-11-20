namespace UserAuthAPI.Models.Dtos
{
    public class GetLoginOTPRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
