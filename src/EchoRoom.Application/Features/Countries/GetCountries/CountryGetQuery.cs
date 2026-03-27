namespace EchoRoom.Application.Features.Countries.GetCountries;

public sealed class CountryGetQuery : IRequest<ApiResult<ICollection<CountryGetResponse>>>
{
}