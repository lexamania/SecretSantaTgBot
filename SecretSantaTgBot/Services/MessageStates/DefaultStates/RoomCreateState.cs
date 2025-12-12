using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class RoomCreateState(MessageBrokerService csm, string parentTitle)
    : SimpleMessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "room_creation";

    protected override string Message => Msgs.RoomCreationEnterTitle;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
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

    private Task SaveTitle(UserTg user, string title)
    {
        UpdateUserState(user, NameParser.JoinArgs(Title, title));
        return NotifyService.SendMessage(user.Id, Msgs.RoomCreationEnterDescription);
    }

    private Task CreateRoom(UserTg user, string title, string description)
    {
        var room = new PartyRoom
        {
            Title = title!,
            PartyDescription = description!,
            Admin = user,
            Users = [new()
            {
                Id = user.Id,
                Username = user.Username,
            }],
        };
        user.AvailableRooms.Add(room);

        DB.Rooms.Insert(room);
        DB.Users.Update(user);

        UpdateUserState(user, default);

        var message = MessageBuilder.BuildCreateRoomMessage(room.Id.ToString());
        return NotifyService.SendMessage(user.Id, message);
    }
}
