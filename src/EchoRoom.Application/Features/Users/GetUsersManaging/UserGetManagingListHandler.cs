namespace EchoRoom.Application.Features.Users.GetUsersManaging;

public sealed class UserGetManagingListHandler(
    IUnitOfWork unitOfWork, 
    ILogger<UserGetManagingListHandler> logger) : IRequestHandler<UserGetManagingListQuery, ApiResult<UserGetManagingListResponse>>
{
    public async Task<ApiResult<UserGetManagingListResponse>> Handle(UserGetManagingListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            UserManagingQueryParams mappedParameters = ToQueryParams(request);

            (ICollection<UserGetManagingResponse> users, int totalCount) = await unitOfWork.UserRepository
                .GetManagingUsersAsync(MapToUserGetManagingResponse, mappedParameters, cancellationToken);

            UserGetManagingListResponse userGetManagingListResponse = new()
            {
                Users = users,
                TotalCount = totalCount
            };

            return ApiResult<UserGetManagingListResponse>.Success(EchoRoomHttpStatusCode.OK, userGetManagingListResponse);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while retrieving users data.");

            return ApiResult<UserGetManagingListResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static readonly Expression<Func<User, UserGetManagingResponse>> MapToUserGetManagingResponse = user => new UserGetManagingResponse
    { 
        Id = user.Id,
        RoleId = user.RoleId,
        CountryId = user.CountryId,
        FullName = user.FullName,
        StreamerNickname = user.StreamerNickname,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        BirthDate = DateTime.SpecifyKind(user.BirthDate, DateTimeKind.Utc),
        CanBeEditedByOtherAdmin = user.CanBeEditedByOtherAdmin,
        CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
        UpdatedAt = DateTime.SpecifyKind(user.UpdatedAt, DateTimeKind.Utc),
        LastLoginAt = DateTime.SpecifyKind(user.LastLoginAt, DateTimeKind.Utc),
    };

    private static UserManagingQueryParams ToQueryParams(UserGetManagingListQuery query) => new()
    {
        SearchQuery = query.SearchQuery,
        RoleIds = query.Filters?.RoleIds ?? [],
        CountryIds = query.Filters?.CountryIds ?? [],
        SortProperty = query.Sorting?.PropertyName ?? UserSortProperty.Id,
        SortDirection = query.Sorting?.Direction ?? SortDirection.ASCENDING,
        Skip = query.SkipItems,
        Take = query.TakeItems
    };
}