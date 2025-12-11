using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class RoomDeleteState(MessageBrokerService csm, string parentTitle)
    : MessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "room_delete";

    protected override string Message => Msgs.ChooseRoom;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        if (MessageParser.IsCommand(msg, out var _, out var _))
            return false;

        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }
    
        var roomId = NameParser.ParseButton(message!).Last();
        var room = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .FirstOrDefault(x => roomId.Equals(x.Id.ToString()));
        
        if (room is null)
        {
            await NotifyService.SendErrorMessage(msg.Chat.Id, Msgs.RoomDoesntExist);
            return true;
        }

        var userIds = room.Users.Select(x => x.Id).ToList();
        var users = DB.Users
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom)
            .Find(x => userIds.Contains(x.Id))
            .ToList();

        foreach (var u in users)
        {
            u.AvailableRooms.Remove(room);
            if (u.SelectedRoom?.Id == room.Id)
            {
                u.SelectedRoom = default;
                u.CurrentState = default;
            }
        }

        DB.Rooms.Delete(room.Id);
        DB.Users.Update(users);
        UpdateUserState(user, default);

        var notifyMessage = MessageBuilder.BuildDeleteRoomMessage(room);
        await NotifyService.NotifyEveryoneInRoom(room, notifyMessage);
        await Csm.UpdateAfterStatusChanged(user);
        return true;
    }

    public override Task StartState(UserTg user, string[] args)
    {
        UpdateUserState(user, Title);
        var buttons = user.AvailableRooms
            .Where(x => x.Admin.Id == user.Id)
            .Select(x => $"{x.Title} {x.Id}")
            .ToArray();
        return NotifyService.SendMessage(user.Id, Message, buttons!);
    }
}
