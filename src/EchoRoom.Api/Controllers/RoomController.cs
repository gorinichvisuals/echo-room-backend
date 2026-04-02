namespace EchoRoom.Api.Controllers;

[Route("api/rooms")]
[ApiController]
public class RoomController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = AuthorizationScopes.AllUsers)]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<RoomGetListResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> GetRooms([FromQuery] RoomGetQuery query, CancellationToken cancellationToken)
    {
        ApiResult<RoomGetListResponse> result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}