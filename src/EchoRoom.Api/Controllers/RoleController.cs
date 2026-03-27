namespace EchoRoom.Api.Controllers;

[Route("api/roles")]
[ApiController]
public class RoleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = AuthorizationScopes.AdminOnly)]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<ICollection<RoleGetResponse>>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        ApiResult<ICollection<RoleGetResponse>> result = await mediator.Send(new RoleGetQuery(), cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}