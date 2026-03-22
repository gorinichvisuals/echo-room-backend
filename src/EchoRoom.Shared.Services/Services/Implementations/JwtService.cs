namespace EchoRoom.Shared.Services.Services.Implementations;

internal sealed class JwtService(IOptions<JWTOptions> options) : IJwtService
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();

    public string CreateAccessToken(int userId, string email, string role, string streamerNickname)
    {
        Claim[] claims =
        [
            new Claim(UserClaims.Id, userId.ToString()!),
            new Claim(UserClaims.Email, email),
            new Claim(UserClaims.Role, role),
            new Claim(UserClaims.StreamerNickname, streamerNickname)
        ];

        JwtSecurityToken token = new(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(options.Value.AccessTokenExpirationDays)),
            signingCredentials: options.Value.SigningCredentials);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(int userId, string email)
    {
        Claim[] claims =
        [
            new(UserClaims.Email, email),
            new(UserClaims.Role, AuthorizationScopes.RefreshToken),
            new(UserClaims.Id, userId.ToString()),
        ];

        JwtSecurityToken jwt = new(
            options.Value.Issuer,
            options.Value.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpirationDays),
            options.Value.SigningCredentials);

        return tokenHandler.WriteToken(jwt);
    }
}