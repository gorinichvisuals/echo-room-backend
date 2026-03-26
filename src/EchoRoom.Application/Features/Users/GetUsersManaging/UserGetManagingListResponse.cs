namespace EchoRoom.Application.Features.Users.GetUsersManaging;

public sealed class UserGetManagingListResponse
{
    public ICollection<UserGetManagingResponse> Users { get; set; } = [];
    public int TotalCount { get; set; }
}