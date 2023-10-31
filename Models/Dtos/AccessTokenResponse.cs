namespace UserAuthAPI.Models.Dtos
{
    public class AccessTokenResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
