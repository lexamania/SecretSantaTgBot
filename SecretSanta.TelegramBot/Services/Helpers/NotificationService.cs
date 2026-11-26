using System.Net;

using SecretSanta.TelegramBot.Attributes;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Localization.Services;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSanta.TelegramBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public sealed class NotificationService(
    TelegramBotService telegramService,
    BotLocalizer localizer)
{
    private TelegramBotClient Bot => telegramService.Bot!;

    /// <summary>
    /// Send message with keyboard removing
    /// </summary>
    public Task SendMessage(long chatId, string message)
    {
        return Bot.SendMessage(chatId,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    /// <summary>
    /// Send message with new keyboard or save old
    /// </summary>
    public Task SendStagedMessage(long chatId, string message, ReplyMarkup? buttons = null)
    {
        return Bot.SendMessage(chatId,
            message,
            parseMode: ParseMode.Html,
            replyMarkup: buttons);
    }

    /// <summary>
    /// Send group of images with caption as one message
    /// </summary>
    public Task SendImages(long chatId, List<string> images, string? caption)
    {
        var fi = images
            .Select(x => new InputMediaPhoto(x))
            .ToArray();
        fi.First().Caption = caption;
        return Bot.SendMediaGroup(chatId, fi);
    }

    /// <summary>
    /// Send message to list of users
    /// </summary>
    public Task NotifyEveryone(long[] users, string message)
    {
        var safeMessage = WebUtility.HtmlEncode(message);
        var tasks = users.Select(user => SendStagedMessage(user, safeMessage)).ToArray();
        return Task.WhenAll(tasks);
    }



    public Task SendNeedAdminRights(long chatId)
        => SendStagedMessage(chatId, localizer.Messages.ErrNeedAdminRights);

    public Task SendWrongText(long chatId)
        => SendStagedMessage(chatId, localizer.Messages.ErrText);

    public Task NotifyEveryoneInRoom(UserEntity sender, PartyRoomEntity room, string message)
    {
        var users = room.Participants
            .Select(u => u.TelegramUserId)
            .Where(x => x != sender.TelegramUserId)
            .ToArray();
        return NotifyEveryone(users, message);
    }
}
