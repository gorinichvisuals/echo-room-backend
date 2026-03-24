namespace EchoRoom.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationAuthentication(configuration);
        services.AddMediatR(mediatRServiceConfiguration => mediatRServiceConfiguration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        ConnectionStringOptions connectionStringOptions = configuration.GetSection(nameof(ConnectionStringOptions)).Get<ConnectionStringOptions>()!;
        services.AddDatabaseInfrastructure(connectionStringOptions.EchoRoomConnectionString);

        services.AddSharedServices();
    }

    private static void AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JWTOptions jwtOptions = configuration.GetSection(nameof(JWTOptions)).Get<JWTOptions>()!;
        SymmetricSecurityKey securityKey = new(Encoding.ASCII.GetBytes(jwtOptions!.JwtSecretKey));

        services.Configure<JWTOptions>(options =>
        {
            options.Issuer = jwtOptions.Issuer;
            options.Audience = jwtOptions.Audience;
            options.AccessTokenExpirationDays = jwtOptions.AccessTokenExpirationDays;
            options.RefreshTokenExpirationDays = jwtOptions.RefreshTokenExpirationDays;
            options.ResetPasswordTokenExpirationDays = jwtOptions.ResetPasswordTokenExpirationDays;
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        });

        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            
            RoleClaimType = UserClaims.Role
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtOptions.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = EchoRoomHttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";

                        ApiResult result = ApiResult.Fail(
                            EchoRoomHttpStatusCode.Unauthorized, "Token is missing, invalid, or expired", ErrorStatusCode.UNAUTHORIZED);

                        return context.Response.WriteAsJsonAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = EchoRoomHttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";

                        ApiResult result = ApiResult.Fail(
                            EchoRoomHttpStatusCode.Forbidden, "You do not have permission to access this resource", ErrorStatusCode.FORBIDDEN);

                        return context.Response.WriteAsJsonAsync(result);
                    }
                };
            });

        services.AddAuthorization();
    }
}