using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomNotifyEveryoneState(MessageBrokerService csm, string parentTitle)
    : MessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_notify";
    private string Message => Msgs.EnterInRoomMessage;

    public override Task StartState(UserEntity user, string[] args)
    {
        UpdateUserState(user, Title);
        return NotifyService.SendMessage(user.Id, Message);
    }

    public override async Task<bool> OnMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        await NotifyService.NotifyEveryoneInRoom(user.SelectedRoom!, message!);
        await NotifyService.SendMessage(user.Id, Msgs.InRoomMessageSend);

        UpdateUserState(user, default);
        await Csm.UpdateAfterStatusChanged(user);
        return true;
    }
}
