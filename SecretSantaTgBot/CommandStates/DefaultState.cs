using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class DefaultState : CommandStateBase
{
    public const string TITLE = "default";

    private readonly Dictionary<string, CommandStateBase> _innerStates;

    public DefaultState(MessageBrokerService csm) : base(csm, TITLE)
    {
        var commands = new List<CommandInfo> {
            new("/start", "START", CommandStart)
            {
                ShowHelp = false
            },
            new("/select_room", Msgs.CommandSelectRoom, CommandSelectRoom),
            new("/create_room", Msgs.CommandCreateRoom, CommandCreateRoom),
            new("/delete_room", Msgs.CommandDeleteRoom, CommandDeleteRoom),
            new("/show_rooms", Msgs.CommandShowRooms, CommandShowRooms)
        };

        foreach (var command in commands)
            Commands.Add(command.Command, command);

        _innerStates = new()
        {
            [RoomSelectState.TITLE] = new RoomSelectState(csm, Title),
            [RoomDeleteState.TITLE] = new RoomDeleteState(csm, Title),
            [RoomCreateState.TITLE] = new RoomCreateState(csm, Title),
        };
    }

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        if (MessageParser.HasNewState(_innerStates, user.CurrentState!, Title, out var innerState))
        {
            if (await innerState!.OnMessage(msg, user))
                return true;
        }

        if (MessageParser.IsCommand(msg, out var command, out var commandArgs))
        {
            if (!Commands.TryGetValue(command!, out var cmd))
                return false;

            await cmd.Callback.Invoke(msg.Chat, user, commandArgs!);
            return true;
        }

        return false;
    }



    private async Task CommandStart(Chat chat, UserTg user, string[] args)
    {
        if (args.Length == 0)
        {
            await CommandHelp(chat, user, args);
            return;
        }

        if (!Guid.TryParse(args[0], out var roomId) || DB.Rooms.FindById(roomId) is not { } room)
        {
            await NotifyService.SendErrorMessage(chat.Id, Msgs.RoomDoesntExist);
            return;
        }

        if (!user.AvailableRooms.Any(x => x.Id == room.Id))
        {
            user.AvailableRooms.Add(room);
            room.Users.Add(new()
            {
                Id = user.Id,
                Username = user.Username
            });

            DB.Rooms.Update(room);
            DB.Users.Update(user);
            await NotifyService.SendMessage(user.Id, Msgs.UserNewParticipation);
        }

        var msg = new Message()
        {
            Chat = chat,
            Text = $"{room.Title} {room.Id}"
        };
        await _innerStates[RoomSelectState.TITLE].OnMessage(msg, user);
    }



    private Task CommandCreateRoom(Chat chat, UserTg user, string[] args)
        => _innerStates[RoomCreateState.TITLE].StartState(user, args);

    private Task CommandSelectRoom(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return NotifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        return _innerStates[RoomSelectState.TITLE].StartState(user, args);
    }

    private Task CommandDeleteRoom(Chat chat, UserTg user, string[] args)
    {
        var rooms = user.AvailableRooms?.Where(x => x.Admin.Id == user.Id).ToList();

        if (rooms is not { Count: > 0 })
            return NotifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        return _innerStates[RoomDeleteState.TITLE].StartState(user, args);
    }

    private Task CommandShowRooms(Chat chat, UserTg user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return NotifyService.SendErrorMessage(chat.Id, Msgs.ZeroRooms);

        var message = MessageBuilder.BuildRoomsInfoMessage(user);
        return NotifyService.SendMessage(chat.Id, message);
    }
}
