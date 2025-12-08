using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.Services;

public class NotificationService
{
    private readonly TelegramBotClient _bot;

    private static MessagesBase Msgs => EnvVariables.Messages;

    public NotificationService(TelegramBotClient bot)
    {
        _bot = bot;
    }

    public Task SendMessage(long chatId, string message)
    {
        return _bot.SendMessage(chatId,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    public Task SendMessage(long chatId, string message, ReplyMarkup buttons)
    {
        return _bot.SendMessage(chatId,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: buttons);
    }

    public Task SendImages(long chatId, List<string> images, string? caption)
    {
        var fi = images
            .Select(x => new InputMediaPhoto(x))
            .ToArray();
        fi.First().Caption = caption;
        return _bot.SendMediaGroup(chatId, fi);
    }

    public Task SendErrorMessage(long chatId, string message)
        => _bot.SendMessage(chatId, message, parseMode: ParseMode.Html);

    public Task SendNotifyMessage(long chatId, string message)
        => _bot.SendMessage(chatId, message, parseMode: ParseMode.Html);



    public Task SendErrorCommandMessage(long chatId)
        => SendErrorMessage(chatId, Msgs.CommandError);

    public Task SendErrorCommandMessage(long chatId, string innerMessage)
        => SendErrorMessage(chatId, $"{Msgs.CommandError}\n\n{innerMessage}");

    public Task NotifyEveryone(PartyRoom room, string message)
    {
        var tasks = new List<Task>();

        foreach (var u in room.Users)
            tasks.Add(SendNotifyMessage(u.Id, message));

        return Task.WhenAll(tasks);
    }

    public Task NotifyEveryoneTheirTarget(PartyRoom room)
    {
        var participants = room.Users;
        var tasks = new List<Task>();

        foreach (var p in participants)
        {
            var target = participants.First(x => x.Id == p.TargetUserId);
            var message = MessageBuilder.BuildTargetMessage(room, target);
            tasks.Add(SendNotifyMessage(p.Id, message));
        }

        return Task.WhenAll(tasks);
    }
}
