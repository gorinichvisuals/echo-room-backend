namespace EchoRoom.Application.Features.Users.ChangeUserRole;

public sealed class UserChangeRoleHandler(
    IUnitOfWork unitOfWork, 
    ILogger<UserChangeRoleHandler> logger) : IRequestHandler<UserChangeRoleCommand, ApiResult<UserChangeRoleResponse>>
{
    public async Task<ApiResult<UserChangeRoleResponse>> Handle(UserChangeRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User? user = await unitOfWork.UserRepository.GetItemWIthIncludes(user => user.Id == request.Id, includes: user => user.Role);

            if (user is null)
                return ApiResult<UserChangeRoleResponse>.Fail(
                    EchoRoomHttpStatusCode.NotFound, "User data not found.", ErrorStatusCode.USER_DATA_NOT_FOUND);

            bool isUserAdmin = user.Role.Name is nameof(UserType.Admin);

            if (isUserAdmin && !user.CanBeEditedByOtherAdmin)
                return ApiResult<UserChangeRoleResponse>.Fail(
                    EchoRoomHttpStatusCode.Forbidden, "Not allowed to change this user.", ErrorStatusCode.USER_ROLE_CHANGE_NOT_ALLOWED);

            bool isNewRoleAdmin = await unitOfWork.RoleRepository
                .Any(role => role.Id == request.RoleId && role.Name == nameof(UserType.Admin));

            if (!isNewRoleAdmin && !request.CanBeEditedByOtherAdmin)
                return ApiResult<UserChangeRoleResponse>.Fail(
                    EchoRoomHttpStatusCode.BadRequest, "CanBeEditedByOtherAdmin can only be false for non-admin users.", ErrorStatusCode.CANNOT_SET_CAN_BE_EDITED_FOR_NON_ADMIN);

            UpdateUser(user, request, isUserAdmin);

            await unitOfWork.Save();

            UserChangeRoleResponse userChangeRoleResponse = new()
            {
                Id = user.Id,
                RoleId = user.RoleId,
                CanBeEditedByOtherAdmin = user.CanBeEditedByOtherAdmin,
                UpdatedAt = user.UpdatedAt
            };

            return ApiResult<UserChangeRoleResponse>.Success(EchoRoomHttpStatusCode.OK, userChangeRoleResponse);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error occurred while changing user role.");

            return ApiResult<UserChangeRoleResponse>.Fail(
                EchoRoomHttpStatusCode.InternalServerError, exception.Message, ErrorStatusCode.INTERNAL_SERVER_ERROR);
        }
    }

    private static void UpdateUser(User user, UserChangeRoleCommand request, bool isUserAdmin)
    {
        if (!isUserAdmin)
            user.CanBeEditedByOtherAdmin = request.CanBeEditedByOtherAdmin;

        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;
    }
}