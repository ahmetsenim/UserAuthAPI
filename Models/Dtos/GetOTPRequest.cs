namespace UserAuthAPI.Models.Dtos
{
    public class GetOTPRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
