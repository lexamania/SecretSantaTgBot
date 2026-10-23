using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomDeleteState: MessageStateBase
{
    public const string TITLE = "room_delete";
    private const string DELETE_TITLE = "delete_confirmation";

    private string Message => Localization.ChooseRoom;
    private readonly ConfirmationState _confirmationState;

    public RoomDeleteState(ServicesContext context, string parentTitle)
        : base(context, NameParser.JoinArgs(parentTitle, TITLE))
    {
        _confirmationState = new ConfirmationState(context, StateUrl, DELETE_TITLE, DeleteConfirmation);
    }

    public override Task PrepareState(UserEntity user, string[] args)
    {
        UpdateUserState(user, StateUrl);
        var buttons = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return Notifications.SendMessage(user.Id, Message, buttons!);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        var parsedStates = StateUtil.ParseState(user.CurrentState, StateUrl);
        if (parsedStates.Length > 0 && parsedStates[0] == DELETE_TITLE)
        {
            if (await _confirmationState.ProcessMessage(msg, user))
                return true;
        }

        if (MessageParser.TryParseCommand(msg, out var _, out var _))
            return false;

        if (!MessageParser.TryParseText(msg, out var message))
        {
            await Notifications.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }
    
        var roomId = NameParser.ParseButton(message!).Last();
        var room = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
        {
            await Notifications.SendErrorMessage(msg.Chat.Id, Localization.RoomDoesntExist);
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
        await Notifications.NotifyEveryoneInRoom(room, notifyMessage);
        await MessageHandler.SendHelpMenu(user);
    }
}
