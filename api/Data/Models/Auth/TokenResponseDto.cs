namespace JwtAuthentication.Data.Models.Auth
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpirationSeconds { get; set; }
    }
}
