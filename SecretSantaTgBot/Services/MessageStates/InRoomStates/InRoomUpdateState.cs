using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomUpdateState(ServiceContainer container, string parentTitle)
    : SimpleMessageStateBase(container, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_update";

    protected override string Message => Msgs.RoomCreationEnterDescription;

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (MessageParser.IsCommand(msg, out var command, out var args))
        {
            await CallRequiredCommand(command!, msg, user, args);
            return true;
        }

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var room = user.SelectedRoom!;
        room.PartyDescription = message!;
        Database.RoomDirectory.Update(room);

        await Notification.SendMessage(user.Id, Msgs.RoomDescriptionUpdated);
        UpdateUserState(user, default);
        return true;
    }
}
