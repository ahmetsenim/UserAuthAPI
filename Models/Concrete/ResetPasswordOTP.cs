namespace UserAuthAPI.Models.Concrete
{
    public class ResetPasswordOTP : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OTP { get; set; }
        public int NumberOfAttempts { get; set; }
        public DateTime ValidityDate { get; set; }
        public bool PasswordReset { get; set; }
    }
}
