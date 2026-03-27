namespace EchoRoom.Application.Features.Countries.GetCountries;

public sealed class CountryGetHandler(
    IUnitOfWork unitOfWork, 
    ILogger<CountryGetHandler> logger) : IRequestHandler<CountryGetQuery, ApiResult<ICollection<CountryGetResponse>>>
{
    public async Task<ApiResult<ICollection<CountryGetResponse>>> Handle(CountryGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<CountryGetResponse> countries = await unitOfWork.CountryRepository
                .GetMappedItems(MapToCountryGetResponse, cancellationToken: cancellationToken);

            return ApiResult<ICollection<CountryGetResponse>>.Success(EchoRoomHttpStatusCode.OK, countries);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while retrieving countries.");

            return ApiResult<ICollection<CountryGetResponse>>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static readonly Expression<Func<Country, CountryGetResponse>> MapToCountryGetResponse = country => new CountryGetResponse
    { 
        Id = country.Id,
        Name = country.Name,
        ISO3Code = country.ISO3Code,
    };
}