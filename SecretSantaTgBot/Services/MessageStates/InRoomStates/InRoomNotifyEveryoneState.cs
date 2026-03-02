using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomNotifyEveryoneState(ServiceContainer container, string parentTitle)
    : MessageStateBase(container, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_notify";
    private string Message => Msgs.EnterInRoomMessage;

    public override Task PrepareState(UserEntity user, string[] args)
    {
        UpdateUserState(user, Title);
        return Notification.SendMessage(user.Id, Message);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        await Notification.NotifyEveryoneInRoom(user.SelectedRoom!, message!);
        await Notification.SendMessage(user.Id, Msgs.InRoomMessageSend);

        UpdateUserState(user, default);
        await MessageBroker.SendHelpMenu(user);
        return true;
    }
}
