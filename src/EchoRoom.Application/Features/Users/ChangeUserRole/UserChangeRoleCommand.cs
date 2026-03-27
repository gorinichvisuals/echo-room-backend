namespace EchoRoom.Application.Features.Users.ChangeUserRole;

public sealed class UserChangeRoleCommand : IRequest<ApiResult<UserChangeRoleResponse>>
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public bool CanBeEditedByOtherAdmin { get; set; }
}