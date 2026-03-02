using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.DefaultStates;

public class RoomDeleteState: MessageStateBase
{
    public const string TITLE = "room_delete";
    private const string DELETE_TITLE = "delete_confirmation";

    private string Message => Msgs.ChooseRoom;
    private readonly ConfirmationState _confirmationState;

    public RoomDeleteState(ServiceContainer container, string parentTitle)
        : base(container, NameParser.JoinArgs(parentTitle, TITLE))
    {
        _confirmationState = new ConfirmationState(container, Title, DELETE_TITLE, DeleteConfirmation);
    }

    public override Task PrepareState(UserEntity user, string[] args)
    {
        UpdateUserState(user, Title);
        var buttons = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return Notification.SendMessage(user.Id, Message, buttons!);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        var parsedStates = NameParser.ParseStateArgs(user.CurrentState, Title);
        if (parsedStates.Length > 0 && parsedStates[0] == DELETE_TITLE)
        {
            if (await _confirmationState.ProcessMessage(msg, user))
                return true;
        }

        if (MessageParser.IsCommand(msg, out var _, out var _))
            return false;

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }
    
        var roomId = NameParser.ParseButton(message!).Last();
        var room = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
        {
            await Notification.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);
            return true;
        }

        await _confirmationState.PrepareState(user, [roomId]);
        return true;
    }

    private async Task DeleteConfirmation(Chat chat, UserEntity user, string[] args)
    {
        var roomId = args[0];
        var room = Database.DeleteRoom(user, roomId);

        var notifyMessage = MessageBuilder.BuildDeleteRoomMessage(room);
        await Notification.NotifyEveryoneInRoom(room, notifyMessage);
        await MessageBroker.SendHelpMenu(user);
    }
}
