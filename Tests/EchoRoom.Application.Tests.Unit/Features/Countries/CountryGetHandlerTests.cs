namespace EchoRoom.Application.Tests.Unit.Features.Countries;

public sealed class CountryGetHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ILogger<CountryGetHandler> _loggerMock;

    private readonly CountryGetHandler _handler;

    public CountryGetHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _loggerMock = Substitute.For<ILogger<CountryGetHandler>>();

        _handler = new CountryGetHandler(_unitOfWorkMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenEverythignIsOk()
    {   
        // Arrange   
        CancellationToken cancellationToken = CancellationToken.None;
        CountryGetQuery query = new();

        ICollection<CountryGetResponse> countries = 
        [
            new()
            {
                Id = 1,
                Name = "United States of America",
                ISO3Code = "USA"
            }
        ];

        _unitOfWorkMock.CountryRepository.GetMappedItems(Arg.Any<Expression<Func<Country, CountryGetResponse>>>())
            .Returns(countries);

        // Act
        ApiResult<ICollection<CountryGetResponse>> result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.ShouldBeAssignableTo<ApiResult<ICollection<CountryGetResponse>>>();
        result.Data.ShouldBeEquivalentTo(countries);
        result.IsSucceed.ShouldBeTrue();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenDatabaseError()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;
        CountryGetQuery query = new();

        _unitOfWorkMock.CountryRepository.GetMappedItems(Arg.Any<Expression<Func<Country, CountryGetResponse>>>())
            .Throws(new Exception("Error occurred while retrieving countries."));

        // Act
        ApiResult<ICollection<CountryGetResponse>> result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.ShouldBeAssignableTo<ApiResult<ICollection<CountryGetResponse>>>();
        result.Data.ShouldBeNull();
        result.IsSucceed.ShouldBeFalse();
        result.StatusCode.ShouldBe(EchoRoomHttpStatusCode.InternalServerError);
        result.ErrorCode.ShouldBe(nameof(ErrorStatusCode.INTERNAL_SERVER_ERROR));
        result.ErrorMessage.ShouldBe("Error occurred while retrieving countries.");
    }
}