using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Services.MessageStates.DefaultStates;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates;

public class DefaultState : MessageStateBase
{
    public const string TITLE = "default";

    private readonly Dictionary<string, MessageStateBase> _innerStates;

    public DefaultState(MessageStateServiceContext context) : base(context, TITLE)
    {
        var commands = new List<CommandModel> {
            new("/select_room", Localization.CommandSelectRoom, CommandSelectRoom),
            new("/create_room", Localization.CommandCreateRoom, CommandCreateRoom),
            new("/delete_room", Localization.CommandDeleteRoom, CommandDeleteRoom),
            new("/show_rooms", Localization.CommandShowRooms, CommandShowRooms)
        };

        foreach (var command in commands)
            Commands.Add(command.Command, command);

        _innerStates = new()
        {
            [RoomSelectState.TITLE] = new RoomSelectState(context, StatePath),
            [RoomDeleteState.TITLE] = new RoomDeleteState(context, StatePath),
            [RoomCreateState.TITLE] = new RoomCreateState(context, StatePath),
        };
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (StateUtil.HasNewState(user.CurrentState!, StatePath, _innerStates, out var innerState))
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



    private Task CommandCreateRoom(Chat chat, UserEntity user, string[] args)
        => _innerStates[RoomCreateState.TITLE].PrepareState(user, args);

    private Task CommandSelectRoom(Chat chat, UserEntity user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return Notifications.SendErrorMessage(chat.Id, Localization.ZeroRooms);

        return _innerStates[RoomSelectState.TITLE].PrepareState(user, args);
    }

    private Task CommandShowRooms(Chat chat, UserEntity user, string[] args)
    {
        if (user.AvailableRooms is not { Count: > 0 })
            return Notifications.SendErrorMessage(chat.Id, Localization.ZeroRooms);

        var message = MessageBuilder.BuildRoomsInfoMessage(user);
        return Notifications.SendMessage(chat.Id, message);
    }

    private Task CommandDeleteRoom(Chat chat, UserEntity user, string[] args)
    {
        var rooms = user.AvailableRooms?.Where(x => x.Admin.Id == user.Id).ToList();

        if (rooms is not { Count: > 0 })
            return Notifications.SendErrorMessage(chat.Id, Localization.ZeroRooms);

        return _innerStates[RoomDeleteState.TITLE].PrepareState(user, args);
    }
}
