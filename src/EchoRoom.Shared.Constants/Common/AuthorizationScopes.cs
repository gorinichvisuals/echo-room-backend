namespace EchoRoom.Shared.Constants.Common;

public static class AuthorizationScopes
{
    public const string RefreshToken = nameof(RefreshToken);
    public const string AllUsers = "Admin, Streamer, BaseUser";
    public const string AdminOnly = "Admin";
}