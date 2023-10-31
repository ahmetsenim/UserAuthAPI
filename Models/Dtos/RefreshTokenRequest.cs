namespace UserAuthAPI.Models.Dtos
{
    public class RefreshTokenRequest
    {
        public int UserId { get; set; }
        public string RefToken { get; set; }
    }
}
