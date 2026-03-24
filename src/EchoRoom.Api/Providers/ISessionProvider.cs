namespace EchoRoom.Api.Providers;

public interface ISessionProvider
{
    int GetUserSessionId();
    string GetUserSessionEmail();
    string GetUserSessionToken();
}