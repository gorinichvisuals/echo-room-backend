namespace EchoRoom.Application.Tests.Unit.Features.Users;

public sealed class UserChangeRoleHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<UserChangeRoleHandler> _loggerMock;

    private readonly UserChangeRoleHandler _handler;

    public UserChangeRoleHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<UserChangeRoleHandler>>();
        
        _handler = new UserChangeRoleHandler(_unitOfWorkMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 2,
            CanBeEditedByOtherAdmin = false,
        };

        User? user = null;

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User,bool>>>(), 
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Returns(user);

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.NotFound);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_DATA_NOT_FOUND));
        result.ErrorMessage.ShouldBe("User data not found.");

        await _unitOfWorkMock.RoleRepository.DidNotReceive()
            .Any(Arg.Any<Expression<Func<Role, bool>>>());

        await _unitOfWorkMock.DidNotReceive()
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenRoleChangeNotAllowed()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = false,
        };

        User user = new() 
        { 
            Id = 1,
            RoleId = 1,
            FullName = "Full Name",
            Email = "test@example.com",
            StreamerNickname = "testStreamer",
            PhoneNumber = "1234567890",
            Password = "hashedPassword",
            CanBeEditedByOtherAdmin = false,
            Role = new Role() 
            { 
                Name = "Admin", 
                Id = 1 
            },
        };

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Returns(user);

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.Forbidden);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.USER_ROLE_CHANGE_NOT_ALLOWED));
        result.ErrorMessage.ShouldBe("Not allowed to change this user.");

        await _unitOfWorkMock.RoleRepository.DidNotReceive()
            .Any(Arg.Any<Expression<Func<Role, bool>>>());

        await _unitOfWorkMock.DidNotReceive()
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenChangeCanBeEditedByOtherAdminForNonAdminUserNotAllowed()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = false,
        };

        User user = new()
        {
            Id = 1,
            RoleId = 3,
            FullName = "Full Name",
            Email = "test@example.com",
            StreamerNickname = "testStreamer",
            PhoneNumber = "1234567890",
            Password = "hashedPassword",
            CanBeEditedByOtherAdmin = true,
            Role = new Role()
            {
                Name = "BaseUser",
                Id = 3
            },
        };

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Returns(user);

        _unitOfWorkMock.RoleRepository.Any(Arg.Any<Expression<Func<Role, bool>>>())
            .Returns(false);

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.BadRequest);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.CANNOT_SET_CAN_BE_EDITED_FOR_NON_ADMIN));
        result.ErrorMessage.ShouldBe("CanBeEditedByOtherAdmin can only be false for non-admin users.");

        await _unitOfWorkMock.DidNotReceive()
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnOK_WhenEverythingIsOk()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = true,
        };

        User user = new()
        {
            Id = 1,
            RoleId = 3,
            FullName = "Full Name",
            Email = "test@example.com",
            StreamerNickname = "testStreamer",
            PhoneNumber = "1234567890",
            Password = "hashedPassword",
            CanBeEditedByOtherAdmin = true,
            Role = new Role()
            {
                Name = "BaseUser",
                Id = 3
            },
        };

        UserChangeRoleResponse expectedResult = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = true
        };

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Returns(user);

        _unitOfWorkMock.RoleRepository.Any(Arg.Any<Expression<Func<Role, bool>>>())
            .Returns(false);

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data!.Id.ShouldBe(expectedResult.Id);
        result.Data!.RoleId.ShouldBe(expectedResult.RoleId);
        result.Data!.CanBeEditedByOtherAdmin.ShouldBe(expectedResult.CanBeEditedByOtherAdmin);

        await _unitOfWorkMock.Received(1)
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnOK_WhenEverythingIsOk_AndDisableAdmin()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = false,
        };

        User user = new()
        {
            Id = 1,
            RoleId = 3,
            FullName = "Full Name",
            Email = "test@example.com",
            StreamerNickname = "testStreamer",
            PhoneNumber = "1234567890",
            Password = "hashedPassword",
            CanBeEditedByOtherAdmin = true,
            Role = new Role()
            {
                Name = "BaseUser",
                Id = 3
            },
        };

        UserChangeRoleResponse expectedResult = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = false
        };

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Returns(user);

        _unitOfWorkMock.RoleRepository.Any(Arg.Any<Expression<Func<Role, bool>>>())
            .Returns(true);

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
        result.Data!.Id.ShouldBe(expectedResult.Id);
        result.Data!.RoleId.ShouldBe(expectedResult.RoleId);
        result.Data!.CanBeEditedByOtherAdmin.ShouldBe(expectedResult.CanBeEditedByOtherAdmin);

        await _unitOfWorkMock.Received(1)
            .Save();
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenDatabaseError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        UserChangeRoleCommand request = new()
        {
            Id = 1,
            RoleId = 1,
            CanBeEditedByOtherAdmin = false,
        };

        _unitOfWorkMock.UserRepository
            .GetItemWIthIncludes(
                Arg.Any<Expression<Func<User, bool>>>(),
                includes: Arg.Any<Expression<Func<User, object>>>())
            .Throws(new Exception("Error occurred while changing user role."));

        // Act
        ApiResult<UserChangeRoleResponse> result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldBeOfType<ApiResult<UserChangeRoleResponse>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while changing user role.");

        await _unitOfWorkMock.RoleRepository.DidNotReceive()
            .Any(Arg.Any<Expression<Func<Role, bool>>>());

        await _unitOfWorkMock.DidNotReceive()
            .Save();
    }
}