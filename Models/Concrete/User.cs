using System.ComponentModel.DataAnnotations.Schema;

namespace UserAuthAPI.Models.Concrete
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
