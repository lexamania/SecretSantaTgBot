using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using SecretSanta.Shared.Configurations;
using SecretSanta.Shared.Storage;
using SecretSanta.TelegramApi.Extensions;
using SecretSanta.TelegramApi.Middleware;
using SecretSanta.TelegramApi.Models;
using SecretSanta.TelegramApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<TelegramBotOptions>()
    .Bind(builder.Configuration.GetSection(TelegramBotOptions.SectionName))
    .Configure(options =>
    {
        options.DatabasePath = Path.GetFullPath(
            builder.Configuration["LiteDb:DatabasePath"] ?? "../SecretSanta.TelegramBot/data/secretsanta.db",
            builder.Environment.ContentRootPath);
    })
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Token),
        "Configuration value 'TelegramBot:Token' is required.")
    .ValidateOnStart();
builder.Services.AddSingleton<ISantaDatabase>(serviceProvider =>
    new LiteDbSantaDatabase(
        serviceProvider.GetRequiredService<IOptions<TelegramBotOptions>>().Value.DatabasePath));
builder.Services.AddRequiredServices();

var app = builder.Build();
var webRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
var webRootProvider = new PhysicalFileProvider(webRootPath);

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/rooms", async Task<IResult> (
    HttpContext context,
    RoomsService roomsService,
    CancellationToken cancellationToken) =>
{
    var result = await roomsService.GetRoomsAsync(context.GetTelegramInitData(), cancellationToken);

    return result.Status switch
    {
        ServiceResultStatus.Success => Results.Ok(result.Value),
        ServiceResultStatus.NotFound => Results.NotFound(new { error = result.Error }),
        ServiceResultStatus.Conflict => Results.Conflict(new { error = result.Error }),
        ServiceResultStatus.Invalid => Results.BadRequest(new { error = result.Error }),
        _ => throw new InvalidOperationException($"Unexpected rooms result status: {result.Status}.")
    };
});

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = webRootProvider
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = webRootProvider
});
app.UseMiddleware<TelegramInitDataMiddleware>();

app.Run();
