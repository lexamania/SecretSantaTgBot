using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class InRoomUpdateState(ServicesContext context, string parentTitle)
    : ContiniousMessageStateBase(context, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_update";

    protected override string Message => Localization.RoomCreationEnterDescription;

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (MessageParser.TryParseCommand(msg, out var command, out var args))
        {
            await CallRequiredCommand(command!, msg, user, args);
            return true;
        }

        if (!MessageParser.TryParseText(msg, out var message))
        {
            await Notifications.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var room = user.SelectedRoom!;
        room.PartyDescription = message!;
        Database.RoomDirectory.Update(room);

        await Notifications.SendMessage(user.Id, Localization.RoomDescriptionUpdated);
        ClearUserState(user);
        return true;
    }
}
