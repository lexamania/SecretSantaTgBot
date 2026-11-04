using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public sealed class NotificationService(
    TelegramBotService telegramService, 
    MessageBuilder msgBuilder)
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

    /// <inheritdoc cref="SendMessage"/>
    public Task SendMessageByKey(long chatId, string key)
        => SendMessage(chatId, msgBuilder.GetByKey(key));

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

    /// <inheritdoc cref="SendStagedMessage"/>
    public Task SendStagedMessageByKey(long chatId, string key, ReplyMarkup? buttons = null)
        => SendStagedMessage(chatId, msgBuilder.GetByKey(key), buttons);

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
        var tasks = users.Select(u => SendStagedMessage(u, message)).ToArray();
        return Task.WhenAll(tasks);
    }



    public Task SendNeedAdminRights(long chatId)
        => SendStagedMessageByKey(chatId, BotContent.Messages.ErrNeedAdminRights);

    public Task SendWrongText(long chatId)
        => SendStagedMessageByKey(chatId, BotContent.Messages.ErrText);

    public Task NotifyEveryoneInRoom(UserEntity sender, PartyRoomEntity room, string message)
    {
        var users = room.Participants
            .Select(u => u.TelegramUserId)
            .Where(x => x != sender.TelegramUserId)
            .ToArray();
        return NotifyEveryone(users, message);
    }
}
