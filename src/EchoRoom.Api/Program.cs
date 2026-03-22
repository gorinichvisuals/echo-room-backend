WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomValidationResponse();
builder.Services.AddEchoRoomCors();
builder.Services.AddProviders();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();