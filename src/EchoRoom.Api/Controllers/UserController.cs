namespace EchoRoom.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IMediator mediator, ISessionProvider sessionProvider) : ControllerBase
{
    [HttpPost]
    [SwaggerResponse(EchoRoomHttpStatusCode.Created, type: typeof(Success<UserGetPersonalInfoResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.BadRequest, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> CreateUser(UserCreateCommand command)
    {
        ApiResult<UserCreateResponse> result = await mediator.Send(command);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("personal-info")]
    [Authorize(Roles = AuthorizationScopes.AllUsers)]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<UserGetPersonalInfoResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.NotFound, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> GetUserPersonalInfo(CancellationToken cancellationToken)
    {
        int userId = sessionProvider.GetUserSessionId();

        UserGetPersonalInfoCommand command = new()
        { 
            UserId = userId 
        };

        ApiResult<UserGetPersonalInfoResponse> result = await mediator.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("managing")]
    [Authorize(Roles = AuthorizationScopes.AdminOnly)]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<UserGetManagingListResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> GetUsersListForManaging([FromQuery] UserGetManagingListQuery query, CancellationToken cancellationToken)
    { 
        ApiResult<UserGetManagingListResponse> result = await mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("change-role")]
    [Authorize(Roles = AuthorizationScopes.AdminOnly)]
    [SwaggerResponse(EchoRoomHttpStatusCode.OK, type: typeof(Success<UserChangeRoleResponse>))]
    [SwaggerResponse(EchoRoomHttpStatusCode.BadRequest, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Unauthorized, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.Forbidden, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.NotFound, type: typeof(Error))]
    [SwaggerResponse(EchoRoomHttpStatusCode.InternalServerError, type: typeof(Error))]
    public async Task<IActionResult> ChangeUserRole(UserChangeRoleCommand command, CancellationToken cancellationToken)
    {
        ApiResult<UserChangeRoleResponse> result = await mediator.Send(command, cancellationToken);

        return StatusCode(result.StatusCode, result);
    }
}