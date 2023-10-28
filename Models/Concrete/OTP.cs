using System.ComponentModel.DataAnnotations.Schema;

namespace UserAuthAPI.Models.Concrete
{
    public class OTP : IEntity
    {
        public int Id { get; set; }
        public int OtpCode { get; set; }
        public string OtpToken { get; set; }
        public int UserId { get; set; }
        public bool IsLoggedIn { get; set; }
        public int NumberOfAttempts { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LoginDate { get; set; }
    }
}
