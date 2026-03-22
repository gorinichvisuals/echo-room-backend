namespace EchoRoom.Shared.Services.Tests.Unit.Services;

public sealed class JwtServiceTests
{
    private readonly IOptions<JWTOptions> _options;
    private readonly JwtService _service;

    public JwtServiceTests()
    {
        _options = Substitute.For<IOptions<JWTOptions>>();

        _options.Value
            .Returns(CreateJwtOptions());

        _service = new JwtService(_options);
    }

    #region CreateAccessToken

    [Fact]
    public void CreateAccessToken_ShouldReturnValidJwt()
    {
        // Arrange
        int userId = 123;
        string email = "test@example.com";
        string role = "Admin";
        string twitchNickname = "Streamer123";

        // Act
        string token = _service.CreateAccessToken(userId, email, role, twitchNickname);

        // Assert
        token.ShouldNotBeNullOrEmpty();

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwt = handler.ReadJwtToken(token);

        jwt.Issuer.ShouldBe(_options.Value.Issuer);
        jwt.Audiences.ShouldContain(_options.Value.Audience);
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Id && c.Value == userId.ToString());
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Email && c.Value == email);
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Role && c.Value == role);
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.StreamerNickname && c.Value == twitchNickname);
    }

    #endregion

    #region CreateRefreshToken

    [Fact]
    public void CreateRefreshToken_ShouldReturnValidJwt()
    {
        // Arrange
        int userId = 123;
        string email = "test@example.com";

        // Act
        string token = _service.CreateRefreshToken(userId, email);

        // Assert
        token.ShouldNotBeNullOrEmpty();

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwt = handler.ReadJwtToken(token);

        jwt.Issuer.ShouldBe(_options.Value.Issuer);
        jwt.Audiences.ShouldContain(_options.Value.Audience);

        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Id && c.Value == userId.ToString());
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Email && c.Value == email);
        jwt.Claims.ShouldContain(c => c.Type == UserClaims.Role && c.Value == AuthorizationScopes.RefreshToken);
    }

    #endregion

    #region Helpers

    private static JWTOptions CreateJwtOptions()
    {
        SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes("super_test_secret_json_web_token_keyyyyyyyyyyyy"));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        return new JWTOptions
        {
            Audience = "testAudience",
            JwtSecretKey = "super_test_secret_json_web_token_keyyyyyyyyyyyy",
            Issuer = "testIssuer",
            AccessTokenExpirationDays = 1,
            RefreshTokenExpirationDays = 1,
            ResetPasswordTokenExpirationDays = 1,
            SigningCredentials = creds
        };
    }

    #endregion
}