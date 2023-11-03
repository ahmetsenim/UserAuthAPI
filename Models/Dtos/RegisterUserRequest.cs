namespace UserAuthAPI.Models.Dtos
{
    public class RegisterUserRequest
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
