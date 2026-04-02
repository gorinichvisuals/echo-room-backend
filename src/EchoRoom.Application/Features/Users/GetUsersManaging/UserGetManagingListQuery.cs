namespace EchoRoom.Application.Features.Users.GetUsersManaging;

public sealed class UserGetManagingListQuery : IRequest<ApiResult<UserGetManagingListResponse>>
{
    public string? SearchQuery { get; set; }
    public int SkipItems { get; set; } = 0;
    public int TakeItems { get; set; } = 20;
    public SortingDto? Sorting { get; set; }
    public UserGetManagingFilters? Filters { get; set; }
}

public sealed class UserGetManagingFilters
{
    public ICollection<int> RoleIds { get; set; } = [];
    public ICollection<int> CountryIds { get; set; } = [];
}

public sealed class SortingDto
{
    public UserSortProperty PropertyName { get; set; }
    public SortDirection Direction { get; set; }
}