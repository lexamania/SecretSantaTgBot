using System.Globalization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SecretSanta.Shared.Configurations;
using SecretSanta.TelegramBot.Configurations;
using SecretSanta.TelegramBot.Services;
using SecretSanta.Shared.Storage;

using Serilog;

CultureInfo.DefaultThreadCurrentCulture =
CultureInfo.DefaultThreadCurrentUICulture =
    CultureInfo.GetCultureInfo("uk");

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

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddOptions<TelegramBotOptions>()
    .Bind(builder.Configuration.GetSection(TelegramBotOptions.SectionName))
    .Configure(options =>
    {
        var databasePath = builder.Configuration["LiteDb:DatabasePath"];
        options.DatabasePath = string.IsNullOrWhiteSpace(databasePath)
            ? string.Empty
            : Path.GetFullPath(databasePath, builder.Environment.ContentRootPath);
    })
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Token),
        "Configuration value 'TelegramBot:Token' is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.DatabasePath),
        "Configuration value 'LiteDb:DatabasePath' is required.")
    .ValidateOnStart();
builder.Services.AddSingleton<BotConfiguration>();
builder.Services.AddSingleton<ISantaDatabase>(provider =>
    new LiteDbSantaDatabase(provider.GetRequiredService<BotConfiguration>().DBPath));

builder.Services.AddRequiredServices();

builder.Services.AddHostedService<HostedService>();

using var app = builder.Build();
var config = app.Services.GetRequiredService<BotConfiguration>();

await app.RunAsync(config.CancellationToken);
