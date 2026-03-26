namespace EchoRoom.Application.Features.Users.GetPersonalInfo;

public sealed class UserGetPersonalInfoHandler(
    IUnitOfWork unitOfWork, 
    ILogger<UserGetPersonalInfoHandler> logger) : IRequestHandler<UserGetPersonalInfoCommand, ApiResult<UserGetPersonalInfoResponse>>
{
    public async Task<ApiResult<UserGetPersonalInfoResponse>> Handle(UserGetPersonalInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            UserGetPersonalInfoResponse? userPersonalInfo = await unitOfWork.UserRepository
                .GetMappedItem(MapToUserPersonalInfo, user => user.Id == request.UserId, cancellationToken);

            return userPersonalInfo is not null
                ? ApiResult<UserGetPersonalInfoResponse>.Success(EchoRoomHttpStatusCode.OK, userPersonalInfo)
                : ApiResult<UserGetPersonalInfoResponse>.Fail(
                    EchoRoomHttpStatusCode.NotFound, "User data not found.", ErrorStatusCode.USER_DATA_NOT_FOUND);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while retrieving user data.");

            return ApiResult<UserGetPersonalInfoResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static readonly Expression<Func<User, UserGetPersonalInfoResponse>> MapToUserPersonalInfo = user => new UserGetPersonalInfoResponse
    {
        Id = user.Id,
        StreamerNickname = user.StreamerNickname,
        FullName = user.FullName,
        Email = user.Email,
        Phone = user.PhoneNumber,
        BirthDate = DateTime.SpecifyKind(user.BirthDate, DateTimeKind.Utc),
        CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc),
        Role = new RoleDto 
        { 
            Id = user.Role.Id,
            Name = user.Role.Name, 
        },
        Country = new CountryDto 
        { 
            Id = user.Country.Id,
            Name = user.Country.Name 
        },
        Balances = user.Balances
            .Select(balance => new BalanceDto 
            {
                Id = balance.Id, 
                Amount = balance.Amount, 
                Currency = balance.Currency 
            }).ToList()
    };
}