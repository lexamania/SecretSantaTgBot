using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.DefaultStates;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates;

public class GlobalState : MessageStateBase
{
    public const string TITLE = "global";

    private readonly RoomSelectState _selectState;

    public GlobalState(MessageBrokerService csm) : base(csm, TITLE)
    {
        var commands = new List<CommandInfo> {
            new("/start", "START", CommandStart)
        };

        foreach (var command in commands)
            Commands.Add(command.Command, command);

        _selectState = new RoomSelectState(csm, Title);
    }

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
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
        await _selectState.OnMessage(msg, user);
    }
}
