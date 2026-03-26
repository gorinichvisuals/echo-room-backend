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

    public string GetUserSessionEmail()
    {
        HttpContext context = httpContextAccessor.HttpContext!;

        return context?.User?.Claims
            .FirstOrDefault(claim => claim.Type == UserClaims.Email)?.Value ?? string.Empty;
    }

    public string GetUserSessionToken()
    {
        HttpContext context = httpContextAccessor.HttpContext!;

        return context!.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
    }

    public string GetUserSessionRoleName()
    {
        HttpContext context = httpContextAccessor.HttpContext!;

        return context?.User?.Claims
            .FirstOrDefault(claim => claim.Type == UserClaims.Role)?.Value ?? string.Empty;   
    }
}