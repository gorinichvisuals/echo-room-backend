namespace EchoRoom.Application.Tests.Unit.Features.Roles;

public sealed class RoleGetHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<RoleGetHandler> _loggerMock;

    private readonly RoleGetHandler _handler;

    public RoleGetHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<RoleGetHandler>>();

        _handler = new RoleGetHandler(_unitOfWorkMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenEverythignIsOk()
    {
        // Arrange   
        CancellationToken cancellationToken = CancellationToken.None;
        RoleGetQuery query = new();

        ICollection<RoleGetResponse> role =
        [
            new()
            {
                Id = 1,
                Name = "Admin",
            },
            new()
            {
                Id = 2,
                Name = "Streamer",
            }
        ];

        _unitOfWorkMock.RoleRepository.GetMappedItems(Arg.Any<Expression<Func<Role, RoleGetResponse>>>())
            .Returns(role);

        // Act
        ApiResult<ICollection<RoleGetResponse>> result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.ShouldBeAssignableTo<ApiResult<ICollection<RoleGetResponse>>>();
        result.Data.ShouldBeEquivalentTo(role);
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenDatabaseError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        RoleGetQuery query = new();

        _unitOfWorkMock.RoleRepository.GetMappedItems(Arg.Any<Expression<Func<Role, RoleGetResponse>>>())
            .Throws(new Exception("Error occurred while retrieving roles."));

        // Act
        ApiResult<ICollection<RoleGetResponse>> result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.ShouldBeAssignableTo<ApiResult<ICollection<RoleGetResponse>>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while retrieving roles.");
    }
}