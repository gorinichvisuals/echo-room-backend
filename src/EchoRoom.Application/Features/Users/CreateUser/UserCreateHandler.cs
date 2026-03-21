namespace EchoRoom.Application.Features.Users.CreateUser;

public sealed class UserCreateHandler(
    IUnitOfWork unitOfWork,
    IJwtService jwtService, 
    ILogger<UserCreateHandler> logger) : IRequestHandler<UserCreateCommand, ApiResult<UserCreateResponse>>
{
    public async Task<ApiResult<UserCreateResponse>> Handle(UserCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            bool countryExists = await unitOfWork.CountryRepository.Any(country => country.Id == request.CountryId);

            if (!countryExists)
                return ApiResult<UserCreateResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "Country not found. Creating user is impossible.", ErrorStatusCode.COUNTRY_DOES_NOT_EXIST);

            User? existingUser = await unitOfWork.UserRepository
                .GetItem(user => user.Email == request.Email 
                              || user.PhoneNumber == request.PhoneNumber 
                              || user.StreamerNickname == request.StreamerNickname);

            if (existingUser?.Email == request.Email)
                return ApiResult<UserCreateResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "User with current email already exists.", ErrorStatusCode.USER_ALREADY_EXISTS_EMAIL);

            if(existingUser?.PhoneNumber == request.PhoneNumber)
                return ApiResult<UserCreateResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "User with current phone number already exists.", ErrorStatusCode.USER_ALREADY_EXISTS_PHONE);

            if(existingUser?.StreamerNickname == request.StreamerNickname)
                return ApiResult<UserCreateResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "User with current nickname already exists.", ErrorStatusCode.USER_ALREADY_EXISTS_NICKNAME);

            User newUser = await CreateUser(request);

            UserCreateResponse tokensDto = CreateResponse(newUser, request.StaySignIn);

            return ApiResult<UserCreateResponse>.Success(EchoRoomHttpStatusCode.Created, tokensDto);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while creating user.");

            return ApiResult<UserCreateResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private async Task<User> CreateUser(UserCreateCommand request)
    {
        Role? userRole = await unitOfWork.RoleRepository.GetItem(role => role.Name == "BaseUser");

        User newUser = new()
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            StreamerNickname = request.StreamerNickname,
            Password = Argon2.Hash(request.Password),
            CountryId = request.CountryId,
            RoleId = userRole!.Id,
            BirthDate = request.BirthDate,
            Role = userRole,
        };

        await unitOfWork.UserRepository.Add(newUser);
        await unitOfWork.Save();

        return newUser;
    }

    private UserCreateResponse CreateResponse(User newUser, bool staySignIn)
        => new()
        {
            Tokens = new TokensDto
            {
                AccessToken = jwtService.CreateAccessToken(newUser.Id, newUser.Email, newUser.Role.Name, newUser.StreamerNickname),
                RefreshToken = staySignIn
                    ? jwtService.CreateRefreshToken(newUser.Id, newUser.Email)
                    : string.Empty,
            },
            StaySignIn = staySignIn
        };
}