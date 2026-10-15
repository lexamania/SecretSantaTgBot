using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Utils;

using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<BotConfiguration>();
builder.Services.AddSingleton<MessagesDictionaryDispatcher>();
builder.Services.AddSingleton<SantaDatabase>();
builder.Services.AddSingleton<MessageBuilder>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "logs/secret-santa-bot.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
            "[{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddHostedService<TelegramBotService>();

builder.Services.AddRequiredServices();

using var app = builder.Build();
var config = app.Services.GetRequiredService<BotConfiguration>();

await app.RunAsync(config.CancellationToken);
