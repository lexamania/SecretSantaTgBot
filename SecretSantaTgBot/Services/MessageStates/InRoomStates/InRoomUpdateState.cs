using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomUpdateState(MessageBrokerService csm, string parentTitle)
    : SimpleMessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_update";

    protected override string Message => Msgs.RoomCreationEnterDescription;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        if (MessageParser.IsCommand(msg, out var command, out var args))
        {
            await CallRequiredCommand(command!, msg, user, args);
            return true;
        }

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var room = user.SelectedRoom!;
        room.PartyDescription = message!;
        DB.Rooms.Update(room);

        UpdateUserState(user, default);
        await NotifyService.SendMessage(user.Id, Msgs.RoomDescriptionUpdated);
        return true;
    }
}
