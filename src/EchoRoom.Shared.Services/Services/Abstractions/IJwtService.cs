namespace EchoRoom.Shared.Services.Services.Abstractions;

public interface IJwtService
{
    string CreateAccessToken(int userId, string email, string role, string streamerNickname);
    string CreateRefreshToken(int userId, string email);
}