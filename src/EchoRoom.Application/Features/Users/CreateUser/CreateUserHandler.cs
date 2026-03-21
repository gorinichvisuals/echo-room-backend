namespace EchoRoom.Application.Features.Users.CreateUser;

public sealed class CreateUserHandler(
    IUnitOfWork unitOfWork,
    IJwtService jwtService, 
    ILogger<CreateUserHandler> logger) : IRequestHandler<CreateUserCommand, ApiResult<CreateUserResponse>>
{
    public async Task<ApiResult<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            bool countryExists = await unitOfWork.CountryRepository.Any(country => country.Id == request.CountryId);

            if (!countryExists)
                return ApiResult<CreateUserResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest,
                    "Country not found. Creating user is impossible!",
                    ErrorStatusCode.COUNTRY_DOES_NOT_EXIST);

            User? existingUser = await unitOfWork.UserRepository
                .GetItem(user => user.Email == request.Email 
                              || user.PhoneNumber == request.PhoneNumber 
                              || user.StreamerNickname == request.StreamerNickname);

            if (existingUser?.Email == request.Email)
                return ApiResult<CreateUserResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, 
                    "User with current email already exists.", 
                    ErrorStatusCode.USER_ALREADY_EXISTS_EMAIL);

            if(existingUser?.PhoneNumber == request.PhoneNumber)
                return ApiResult<CreateUserResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, 
                    "User with current phone number already exists.", 
                    ErrorStatusCode.USER_ALREADY_EXISTS_PHONE);

            if(existingUser?.StreamerNickname == request.StreamerNickname)
                return ApiResult<CreateUserResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest,
                    "User with current nickname already exists.",
                    ErrorStatusCode.USER_ALREADY_EXISTS_NICKNAME);

            User newUser = await CreateUser(request);

            CreateUserResponse tokensDto = CreateTokens(newUser.Id, newUser.Email, newUser.Role.Name, newUser.StreamerNickname, request.StaySignIn);

            return ApiResult<CreateUserResponse>.Success(EchoRoomHttpStatusCode.Created, tokensDto);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while creating user");

            return ApiResult<CreateUserResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, 
                exception.Message, 
                ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private async Task<User> CreateUser(CreateUserCommand request)
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

    private CreateUserResponse CreateTokens(int userId, string email, string role, string twitchNickname, bool staySignIn)
        => new()
        {
            Tokens = new TokensDto
            {
                AccessToken = jwtService.CreateAccessToken(userId, email, role, twitchNickname),
                RefreshToken = staySignIn
                    ? jwtService.CreateRefreshToken(userId, email)
                    : string.Empty,
            },
            StaySignIn = staySignIn
        };
}