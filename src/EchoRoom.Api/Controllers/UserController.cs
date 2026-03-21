namespace EchoRoom.Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        ApiResult<CreateUserResponse> result = await mediator.Send(command);

        return StatusCode(result.StatusCode, result);
    }
}