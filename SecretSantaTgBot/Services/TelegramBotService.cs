using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Configurations;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Services.Brokers;
using SecretSantaTgBot.Services.Helpers;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

[RequiredService(InjectionType.Singleton)]
public class TelegramBotService(
    UserLanguageService languageService,
    LocalizationService localizationService,
    IStringLocalizer<BotContent> texts,
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
        foreach (var language in languageService.SupportedLanguages)
        {
            using (localizationService.UseCulture(language))
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
            Description = texts[BotContent.Commands.Start].Value
        },
        new BotCommand
        {
            Command = BotCommandsList.Help,
            Description = texts[BotContent.Commands.Help].Value
        },
        new BotCommand
        {
            Command = BotCommandsList.Stop,
            Description = texts[BotContent.Commands.Stop].Value
        }
    ];
}
