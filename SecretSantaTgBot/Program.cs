using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "logs/secret-santa-bot-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
            "[{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddSingleton<BotConfiguration>();
builder.Services.AddSingleton<LocalizationDispatcher>();
builder.Services.AddSingleton<SantaDatabase>();

builder.Services.AddRequiredServices();

builder.Services.AddHostedService<HostedService>();

using var app = builder.Build();
var config = app.Services.GetRequiredService<BotConfiguration>();

await app.RunAsync(config.CancellationToken);
