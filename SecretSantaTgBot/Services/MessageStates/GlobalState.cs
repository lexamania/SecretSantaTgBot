using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.DefaultStates;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates;

public class GlobalState : MessageStateBase
{
    public const string TITLE = "global";

    private readonly Dictionary<string, MessageStateBase> _innerStates;

    public GlobalState(ServiceContainer container) : base(container, TITLE, setHelp: false)
    {
        var commands = new List<CommandInfo> {
            new("/start", "START", CommandStart)
        };

        foreach (var command in commands)
            Commands.Add(command.Command, command);

        _innerStates = new()
        {
            [RoomSelectState.TITLE] = new RoomSelectState(container, Title)
        };
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (MessageParser.HasNewState(_innerStates, user.CurrentState!, Title, out var innerState))
        {
            if (await innerState!.ProcessMessage(msg, user))
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



    private async Task CommandStart(Chat chat, UserEntity user, string[] args)
    {
        if (args.Length == 0)
        {
            await CommandHelp(chat, user, args);
            return;
        }

        if (!Guid.TryParse(args[0], out var roomId)
            || Database.RoomDirectory.GetById(roomId) is not { } room)
        {
            await Notification.SendErrorMessage(chat.Id, Msgs.RoomDoesntExist);
            return;
        }

        if (!user.AvailableRooms.Any(x => x.Id == room.Id))
        {
            Database.JoinRoom(user, room);
            await Notification.SendMessage(user.Id, Msgs.UserNewParticipation);
        }

        var msg = new Message()
        {
            Chat = chat,
            Text = $"{room.Title} {room.Id}"
        };
        await _innerStates[RoomSelectState.TITLE].ProcessMessage(msg, user);
    }
}
