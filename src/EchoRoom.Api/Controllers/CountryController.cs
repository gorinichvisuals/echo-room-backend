namespace EchoRoom.Api.Controllers;

[Route("api/countries")]
[ApiController]
public class CountryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<ICollection<CountryGetResponse>>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
    {
        ApiResult<ICollection<CountryGetResponse>> result = await mediator.Send(new CountryGetQuery(), cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}