namespace EchoRoom.Api.Providers;

internal sealed class SessionProvider(IHttpContextAccessor httpContextAccessor) : ISessionProvider
{
    public int GetUserSessionId()
    {
        HttpContext context = httpContextAccessor.HttpContext!;

        if (context is null)
            return default;

        _ = int.TryParse(context.User.Claims.FirstOrDefault(claim => claim.Type == UserClaims.Id)?.Value,
            out int userId);

        return userId;
    }
}