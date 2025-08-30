namespace JwtAuthentication.Data.Models.Auth
{
    public record RefreshTokenRequestDto(Guid UserId, string RefreshToken);
}
