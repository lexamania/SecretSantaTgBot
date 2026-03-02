using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class RoomCreateState(MessageBrokerService csm, string parentTitle)
    : SimpleMessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "room_creation";

    protected override string Message => Msgs.RoomCreationEnterTitle;

    public override async Task<bool> OnMessage(Message msg, UserEntity user)
    {
        if (MessageParser.IsCommand(msg, out var command, out var tempArgs))
        {
            await CallRequiredCommand(command!, msg, user, tempArgs);
            return true;
        }

        var args = NameParser.ClearState(user.CurrentState, Title);
        var enterMessage = args is null || args.Length == 0
            ? Msgs.RoomCreationEnterTitle
            : Msgs.RoomCreationEnterDescription;

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, enterMessage);
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
        UpdateUserState(user, NameParser.JoinArgs(Title, title));
        return NotifyService.SendMessage(user.Id, Msgs.RoomCreationEnterDescription);
    }

    private Task CreateRoom(UserEntity user, string title, string description)
    {
        var room = DB.RoomDirectory.Create(user, title, description);
        user.AvailableRooms.Add(room);
        DB.UserDirectory.UpdateWithClearState(user);

        var message = MessageBuilder.BuildCreateRoomMessage(room.Id.ToString());
        return NotifyService.SendMessage(user.Id, message);
    }
}
