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

    public static void AddProviders(this IServiceCollection services) 
    {
        services.AddScoped<ISessionProvider, SessionProvider>();
        services.AddHttpContextAccessor();
    }

    public static void AddEchoRoomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Echo Room API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert: {token}"
            });
        });
    }

    public static void UseEchoRoomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }
}