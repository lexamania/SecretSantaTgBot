using SecretSanta.TelegramBot.Enums;
using Microsoft.Extensions.DependencyInjection;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.TelegramBot.Configurations;
using SecretSanta.TelegramBot.Services.Brokers;

using Telegram.Bot;
using Telegram.Bot.Types;
using SecretSanta.Localization.Services;

namespace SecretSanta.TelegramBot.Services;

[RequiredService(InjectionType.Singleton)]
public class TelegramBotService(
    CultureSwitcher cultureSwitcher,
    BotLocalizer localizer,
    IServiceProvider provider)
{
    public TelegramBotClient? Bot { get; private set; }

    public async Task Initialize()
    {
        if (Bot is not null) return;

        var config = provider.GetRequiredService<BotConfiguration>();

        Bot = new TelegramBotClient(
            config.BotToken,
            cancellationToken: config.CancellationToken);

        await RegisterDefaultAsync(config.CancellationToken);
        await RegisterAsync(config.CancellationToken);

        Bot.OnMessage += provider.GetRequiredService<MessageBroker>().OnMessage;
        Bot.OnUpdate += provider.GetRequiredService<QueryBroker>().OnUpdate;
        Bot.OnError += provider.GetRequiredService<ErrorBroker>().OnError;
    }

    public Task<User> VerifyBotInfo() => Bot!.GetMe();

    private Task RegisterDefaultAsync(CancellationToken ct)
        => Bot!.SetMyCommands(GetCommands(), cancellationToken: ct);

    private async Task RegisterAsync(CancellationToken ct)
    {
        foreach (var language in LanguageManager.Supported)
        {
            using (cultureSwitcher.UseCulture(language))
            {
                await Bot!.SetMyCommands(
                    GetCommands(),
                    languageCode: language,
                    cancellationToken: ct);
            }
        }
    }

    private BotCommand[] GetCommands() =>
    [
        new BotCommand
        {
            Command = BotCommandsList.Start,
            Description = BotCommandsList.Descriptions.Get(BotCommandsList.Start, localizer)!
        },
        new BotCommand
        {
            Command = BotCommandsList.Help,
            Description = BotCommandsList.Descriptions.Get(BotCommandsList.Help, localizer)!
        },
        new BotCommand
        {
            Command = BotCommandsList.Stop,
            Description = BotCommandsList.Descriptions.Get(BotCommandsList.Stop, localizer)!
        }
    ];
}
