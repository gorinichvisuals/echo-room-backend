namespace EchoRoom.Application.Features.Users.GetPersonalInfo;

public sealed class UserGetPersonalInfoCommand : IRequest<ApiResult<UserGetPersonalInfoResponse>>
{
    public int UserId { get; set; }
}