namespace EchoRoom.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IMediator mediator, ISessionProvider sessionProvider) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserCreateCommand command)
    {
        ApiResult<UserCreateResponse> result = await mediator.Send(command);

        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("personal-info")]
    [Authorize(Roles = AuthorizationScopes.AllUsers)]
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
}