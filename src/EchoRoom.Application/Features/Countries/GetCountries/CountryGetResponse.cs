namespace EchoRoom.Application.Features.Countries.GetCountries;

public sealed class CountryGetResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ISO3Code { get; set; }
}