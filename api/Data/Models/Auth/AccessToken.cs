namespace JwtAuthentication.Data.Models.Auth
{
    public class AccessToken
    {
        public string Token { get; set; } = string.Empty;
        public int ExpirationSeconds { get; set; }
    }
}
