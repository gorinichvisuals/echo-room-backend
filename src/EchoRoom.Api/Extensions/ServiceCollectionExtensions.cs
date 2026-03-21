namespace EchoRoom.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCustomValidationResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                ICollection<string> validationErrors = context.ModelState
                    .Where(entry => entry.Value!.Errors.Count > 0)
                    .SelectMany(entry => entry.Value!.Errors)
                    .Select(error => error.ErrorMessage)
                    .ToList();

                string errorMessage = string.Join(
                    Environment.NewLine,
                    validationErrors.Select(message => $"- {message}"));

                ApiResult<object> result = ApiResult<object>.Fail(
                    EchoRoomHttpStatusCode.BadRequest,
                    errorMessage,
                    ErrorStatusCode.VALIDATION_ERROR);

                return new BadRequestObjectResult(result);
            };
        });
    }

    public static void AddEchoRoomCors(this IServiceCollection services)
    {
        services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins(
                                "http://localhost:3000");
                    });
            });
    }
}