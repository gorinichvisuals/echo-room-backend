namespace EchoRoom.Application.Tests.Unit.Features.Users;

public sealed class CreateUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IJwtService _jwtServiceMock;
    private readonly ILogger<UserCreateHandler> _loggerMock;

    private readonly UserCreateHandler _handler;

    public CreateUserHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _loggerMock = Substitute.For<ILogger<UserCreateHandler>>();

        _handler = new UserCreateHandler(_unitOfWorkMock, _jwtServiceMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenCountryDoesNotExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = false;

        UserCreateCommand request = new() 
        { 
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.COUNTRY_DOES_NOT_EXIST));
        result.ErrorMessage.ShouldBe("Country not found. Creating user is impossible.");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenUserWithSameEmailExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = true;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        User existingUser = new()
        {
            FullName = "Full Name",
            StreamerNickname = "TestNickname",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            Password = "hashedPassword",
        };

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        _unitOfWorkMock.UserRepository.GetItem(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(existingUser);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_ALREADY_EXISTS_EMAIL));
        result.ErrorMessage.ShouldBe("User with current email already exists.");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenUserWithSamePhoneExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = true;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        User existingUser = new()
        {
            FullName = "Full Name",
            StreamerNickname = "TestNickname",
            Email = "test_test@gmail.com",
            PhoneNumber = "+380663036999",
            Password = "hashedPassword",
        };

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        _unitOfWorkMock.UserRepository.GetItem(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(existingUser);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_ALREADY_EXISTS_PHONE));
        result.ErrorMessage.ShouldBe("User with current phone number already exists.");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenUserWithSameNicknameExists()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = true;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test_test@gmail.com",
            PhoneNumber = "+380663036990",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        User existingUser = new()
        {
            FullName = "Full Name",
            StreamerNickname = "TestNickname",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            Password = "hashedPassword",
        };

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        _unitOfWorkMock.UserRepository.GetItem(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(existingUser);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_ALREADY_EXISTS_NICKNAME));
        result.ErrorMessage.ShouldBe("User with current nickname already exists.");
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WithoutStaySignIn()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = true;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test_test@gmail.com",
            PhoneNumber = "+380663036990",
            StreamerNickname = "TestNickname_2",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = false
        };

        User existingUser = new()
        {
            FullName = "Full Name",
            StreamerNickname = "TestNickname",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            Password = "hashedPassword",
        };

        Role role = new() 
        { 
            Id = 1, 
            Name = "BaseUser" 
        };

        UserCreateResponse expectedResult = new()
        {
            Tokens = new()
            {
                AccessToken = "accessToken",
                RefreshToken = string.Empty
            },
            StaySignIn = false
        };

        string accessToken = "accessToken";
        string refreshToken = string.Empty;

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        _unitOfWorkMock.UserRepository.GetItem(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(existingUser);

        _unitOfWorkMock.RoleRepository.GetItem(Arg.Any<Expression<Func<Role, bool>>>())
            .Returns(role);

        _jwtServiceMock.CreateAccessToken(Arg.Any<int>(), request.Email, Arg.Any<string>(), request.StreamerNickname)
            .Returns(accessToken);

        _jwtServiceMock.CreateRefreshToken(Arg.Any<int>(), request.Email)
            .Returns(refreshToken);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeEquivalentTo(expectedResult);
        result.IsSucceed.ShouldBeTrue();

        await _unitOfWorkMock.UserRepository.Received(1)
            .Add(Arg.Any<User>());

        await _unitOfWorkMock.Received(1)
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WithStaySignIn()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        bool countryExists = true;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test_test@gmail.com",
            PhoneNumber = "+380663036990",
            StreamerNickname = "TestNickname_2",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        User existingUser = new()
        {
            FullName = "Full Name",
            StreamerNickname = "TestNickname",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            Password = "hashedPassword",
        };

        Role role = new()
        {
            Id = 1,
            Name = "BaseUser"
        };

        UserCreateResponse expectedResult = new()
        {
            Tokens = new()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken"
            },
            StaySignIn = true
        };

        string accessToken = "accessToken";
        string refreshToken = "refreshToken";

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Returns(countryExists);

        _unitOfWorkMock.UserRepository.GetItem(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(existingUser);

        _unitOfWorkMock.RoleRepository.GetItem(Arg.Any<Expression<Func<Role, bool>>>())
            .Returns(role);

        _jwtServiceMock.CreateAccessToken(Arg.Any<int>(), request.Email, Arg.Any<string>(), request.StreamerNickname)
            .Returns(accessToken);

        _jwtServiceMock.CreateRefreshToken(Arg.Any<int>(), request.Email)
            .Returns(refreshToken);

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeEquivalentTo(expectedResult);
        result.IsSucceed.ShouldBeTrue();

        await _unitOfWorkMock.UserRepository.Received(1)
            .Add(Arg.Any<User>());

        await _unitOfWorkMock.Received(1)
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenCoutryRepositoryReturnsDbError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserCreateCommand request = new()
        {
            FullName = "Full Name",
            Email = "test@gmail.com",
            PhoneNumber = "+380663036999",
            StreamerNickname = "TestNickname",
            Password = "mintest",
            BirthDate = new DateTime(2000, 1, 1, 00, 00, 00, DateTimeKind.Utc),
            CountryId = 1,
            StaySignIn = true
        };

        _unitOfWorkMock.CountryRepository.Any(Arg.Any<Expression<Func<Country, bool>>>())
            .Throws(new Exception("Error occurred while creating user."));

        // Act
        ApiResult<UserCreateResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserCreateResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while creating user.");
    }
}