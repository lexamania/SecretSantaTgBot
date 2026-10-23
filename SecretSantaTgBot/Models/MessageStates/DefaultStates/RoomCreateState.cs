using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomCreateState(ServicesContext context, string parentTitle)
    : ContiniousMessageStateBase(context, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "room_creation";

    protected override string Message => Localization.RoomCreationEnterTitle;

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (MessageParser.TryParseCommand(msg, out var command, out var tempArgs))
        {
            await CallRequiredCommand(command!, msg, user, tempArgs);
            return true;
        }

        var args = StateUtil.ClearState(user.CurrentState, StateUrl);
        var enterMessage = args is null || args.Length == 0
            ? Localization.RoomCreationEnterTitle
            : Localization.RoomCreationEnterDescription;

        if (!MessageParser.TryParseText(msg, out var message))
        {
            await Notifications.SendErrorCommandMessage(msg.Chat.Id, enterMessage);
            return true;
        }

        if (args is null || args.Length == 0)
            await SaveTitle(user, message!);
        else
            await CreateRoom(user, args, message!);

        return true;
    }

    private Task SaveTitle(UserEntity user, string title)
    {
        UpdateUserState(user, NameParser.JoinArgs(StateUrl, title));
        return Notifications.SendMessage(user.Id, Localization.RoomCreationEnterDescription);
    }

    private Task CreateRoom(UserEntity user, string title, string description)
    {
        var room = Database.RoomDirectory.Create(user, title, description);
        user.AvailableRooms.Add(room);
        ClearUserState(user);

        var message = MessageBuilder.BuildCreateRoomMessage(room.Id.ToString());
        return Notifications.SendMessage(user.Id, message);
    }
}
