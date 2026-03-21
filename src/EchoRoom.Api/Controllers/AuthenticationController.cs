namespace EchoRoom.Api.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        ApiResult<LoginResponse> result = await mediator.Send(command);

        return StatusCode(result.StatusCode, result);
    }
}