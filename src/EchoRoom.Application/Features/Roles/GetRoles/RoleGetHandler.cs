namespace EchoRoom.Application.Features.Roles.GetRoles;

public sealed class RoleGetHandler(
    IUnitOfWork unitOfWork, 
    ILogger<RoleGetHandler> logger) : IRequestHandler<RoleGetQuery, ApiResult<ICollection<RoleGetResponse>>>
{
    public async Task<ApiResult<ICollection<RoleGetResponse>>> Handle(RoleGetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<RoleGetResponse> roles = await unitOfWork.RoleRepository
                .GetMappedItems(MapToRoleResponse, cancellationToken: cancellationToken);

            return ApiResult<ICollection<RoleGetResponse>>.Success(EchoRoomHttpStatusCode.OK, roles);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while retrieving roles.");

            return ApiResult<ICollection<RoleGetResponse>>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static readonly Expression<Func<Role, RoleGetResponse>> MapToRoleResponse = role => new RoleGetResponse 
    { 
        Id = role.Id,
        Name = role.Name 
    };
}