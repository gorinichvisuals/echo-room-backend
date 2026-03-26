namespace EchoRoom.Database.Infrastructure.QueryParams;

public sealed class UserManagingQueryParams
{
    public string? SearchQuery { get; set; }
    public ICollection<int> RoleIds { get; set; } = [];
    public ICollection<int> CountryIds { get; set; } = [];
    public UserSortProperty SortProperty { get; set; }
    public SortDirection SortDirection { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}