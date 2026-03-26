namespace EchoRoom.Database.Infrastructure.Extensions;

public static class UserSortingExtensions
{
    public static IQueryable<User> ApplySorting(
        this IQueryable<User> query,
        UserSortProperty property,
        SortDirection direction)
    {
        Expression<Func<User, object>> keySelector = property switch
        {
            UserSortProperty.Username => user => user.StreamerNickname,
            UserSortProperty.DateOfBirth => user => user.BirthDate,
            UserSortProperty.CreatedAt => user => user.CreatedAt,
            UserSortProperty.LastLoginAt => user => user.LastLoginAt,
            UserSortProperty.Country => user => user.Country.Name!,
            UserSortProperty.Role => user => user.Role.Name!,
            _ => user => user.Id
        };

        return direction == SortDirection.ASCENDING
            ? query.OrderBy(keySelector)
            : query.OrderByDescending(keySelector);
    }
}