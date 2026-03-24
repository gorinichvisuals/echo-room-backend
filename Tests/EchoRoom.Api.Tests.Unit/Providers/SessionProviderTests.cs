namespace EchoRoom.Api.Tests.Unit.Providers;

public sealed class SessionProviderTests
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ClaimsPrincipal _user;
    private readonly DefaultHttpContext _httpContext;

    private readonly SessionProvider _sut;

    public SessionProviderTests()
    {
        _contextAccessor = Substitute.For<IHttpContextAccessor>();
        _sut = new SessionProvider(_contextAccessor);

        _user = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(UserClaims.Email, "test@example.com"),
                new Claim(UserClaims.Id, "123")
            ]));

        _httpContext = new DefaultHttpContext { User = _user };

        _contextAccessor.HttpContext
            .Returns(_httpContext);
    }

    #region GetUserSessionId

    [Fact]
    public void GetUserSessionId_ShouldReturnUserId_WhenClaimExists()
    {
        // Act
        var result = _sut.GetUserSessionId();

        // Assert
        result.ShouldBe(123);
    }

    [Fact]
    public void GetUserSessionId_ShouldReturnDefault_WhenClaimDoesNotExist()
    {
        // Arrange
        var userWithoutId = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(UserClaims.Email, "test@example.com")
            ]));

        _httpContext.User = userWithoutId;

        // Act
        var result = _sut.GetUserSessionId();

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public void GetUserSessionId_ShouldReturnDefault_WhenClaimIsNotInt()
    {
        // Arrange
        var userWithInvalidId = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(UserClaims.Id, "invalid-int")
            ]));

        _httpContext.User = userWithInvalidId;

        // Act
        var result = _sut.GetUserSessionId();

        // Assert
        result.ShouldBe(0);
    }

    [Fact]
    public void GetUserSessionId_ShouldReturnDefault_WhenHttpContextIsNull()
    {
        // Arrange
        _contextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        var result = _sut.GetUserSessionId();

        // Assert
        result.ShouldBe(0);
    }

    #endregion

    #region GetUserSessionEmail

    [Fact]
    public void GetUserSessionEmail_ShouldReturnEmail_WhenEmailClaimExists()
    {
        // Act
        var result = _sut.GetUserSessionEmail();

        // Assert
        result.ShouldBeEquivalentTo("test@example.com");
    }

    #endregion

    #region GetUserSessionToken

    [Fact]
    public void GetUserSessionToken_ShouldReturnToken_WhenAuthorizationHeaderExists()
    {
        // Arrange
        _httpContext.Request.Headers.Authorization = "Bearer test-token";

        // Act
        var result = _sut.GetUserSessionToken();

        // Assert
        result.ShouldBeEquivalentTo("test-token");
    }

    #endregion
}