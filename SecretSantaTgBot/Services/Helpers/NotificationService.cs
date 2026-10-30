using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Localization;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public sealed class NotificationService(
    TelegramBotService telegramService, 
    LocalizationDispatcher msgDspt)
{
    private TelegramBotClient Bot => telegramService.Bot;

    public Task SendMessage(long chatId, string message, ReplyMarkup? buttons = null)
    {
        return Bot.SendMessage(chatId,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: buttons ?? new ReplyKeyboardRemove());
    }

    public Task SendImages(long chatId, List<string> images, string? caption)
    {
        var fi = images
            .Select(x => new InputMediaPhoto(x))
            .ToArray();
        fi.First().Caption = caption;
        return Bot.SendMediaGroup(chatId, fi);
    }

    public Task SendErrorMessage(long chatId, string message)
        => Bot.SendMessage(chatId, message, parseMode: ParseMode.Html);

    public Task SendNotifyMessage(long chatId, string message, bool disableNotification = false)
        => Bot.SendMessage(chatId, message, parseMode: ParseMode.Html, disableNotification: disableNotification);

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
        => SendErrorMessage(chatId, msgDspt.Get().CommandError);

    public Task SendErrorCommandMessage(long chatId, string innerMessage)
        => SendErrorMessage(chatId, $"{msgDspt.Get().CommandError}\n\n{innerMessage}");

    public Task NotifyEveryoneInRoom(PartyRoomEntity room, string message)
    {
        var users = room.Participants.Select(u => u.TelegramUserId).ToArray();
        return NotifyEveryone(users, message);
    }
}
