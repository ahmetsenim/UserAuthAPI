namespace UserAuthAPI.Models.Concrete
{
    public class UserClaim : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClaimId { get; set; }
    }
}
