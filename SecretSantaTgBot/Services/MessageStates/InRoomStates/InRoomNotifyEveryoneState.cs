using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomNotifyEveryoneState(ServicesContext context, string parentTitle)
    : MessageStateBase(context, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_notify";
    private string Message => Localization.EnterInRoomMessage;

    public override Task PrepareState(UserEntity user, string[] args)
    {
        UpdateUserState(user, StatePath);
        return Notifications.SendMessage(user.Id, Message);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notifications.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        await Notifications.NotifyEveryoneInRoom(user.SelectedRoom!, message!);
        await Notifications.SendMessage(user.Id, Localization.InRoomMessageSend);

        ClearUserState(user);
        await MessageHandler.SendHelpMenu(user);
        return true;
    }
}
