namespace EchoRoom.Api.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<LoginResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.BadRequest, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        ApiResult<LoginResponse> result = await mediator.Send(command);

        return StatusCode(result.StatusCode, result);
    }
}