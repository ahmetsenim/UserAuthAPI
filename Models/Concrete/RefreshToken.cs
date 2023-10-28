using System.ComponentModel.DataAnnotations.Schema;

namespace UserAuthAPI.Models.Concrete
{
    public class RefreshToken : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefToken { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsValid { get; set; }
    }
}
