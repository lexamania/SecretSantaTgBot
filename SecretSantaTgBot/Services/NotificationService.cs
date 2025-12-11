using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage.Models;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.Services;

public class NotificationService(TelegramBotClient bot)
{
    private readonly TelegramBotClient _bot = bot;
    private static MessagesBase Msgs => EnvVariables.Messages;

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

    public Task SendNotifyMessage(long chatId, string message, bool disableNotification = false)
        => _bot.SendMessage(chatId, message, parseMode: ParseMode.Html, disableNotification: disableNotification);

    public Task NotifyEveryone(long[] users, string message, bool disableNotification = false)
    {
        var tasks = users.Select(u => SendNotifyMessage(u, message, disableNotification)).ToArray();
        return Task.WhenAll(tasks);
    }

    public Task NotifyEveryone(List<(long Id, string Msg)> messages, bool disableNotification = false)
    {
        var tasks = messages.Select(msg => SendNotifyMessage(msg.Id, msg.Msg, disableNotification));
        return Task.WhenAll(tasks);
    }



    public Task SendErrorCommandMessage(long chatId)
        => SendErrorMessage(chatId, Msgs.CommandError);

    public Task SendErrorCommandMessage(long chatId, string innerMessage)
        => SendErrorMessage(chatId, $"{Msgs.CommandError}\n\n{innerMessage}");

    public Task NotifyEveryoneInRoom(PartyRoom room, string message)
    {
        var users = room.Users.Select(u => u.Id).ToArray();
        return NotifyEveryone(users, message);
    }
}
