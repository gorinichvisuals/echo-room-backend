namespace EchoRoom.Application.Features.Users.ChangeUserRole;

public sealed class UserChangeRoleResponse
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public bool CanBeEditedByOtherAdmin { get; set; }
    public DateTime UpdatedAt { get; set; }
}