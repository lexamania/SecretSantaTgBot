using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;
using SecretSantaTgBot.Models.MessageStates;
using SecretSantaTgBot.Extensions;

namespace SecretSantaTgBot.Models.Commands.General;

public class StartBotCommand(
    ServicesContext services,
    DefaultState defaultState)
    : BotCommandBase(BotCommandsList.Start)
{
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var args = context.Message.CommandMsg!.Args;
        var user = context.User;

        if (args.Length == 0)
        {
            services.Database.Users.ClearState(context.User);
            await context.GlobalMessageHandler.SendHelpMenu(context.User);
            return;
        }

        if (!TryGetRoom(args[0], out var room))
        {
            await services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.RoomDoesntExist);
            return;
        }

        var task = user.GetAvailableRoom(room!.Id) is null
            ? RegistrateRoom(context, room!)
            : RedirectToRoom(chat, context, room!);
        await task;
    }

    private Task RegistrateRoom(MessageContext context, PartyRoomEntity room)
    {
        context.State.ReplaceState(StatesList.Default);
        context.State.StateObject.Arguments[StateArgsList.RoomId] = room.Id.ToString();
        return defaultState.ReadOnlyStates[StatesList.RoomRegistration].PrepareState(context);
    }

    private Task RedirectToRoom(Chat chat, MessageContext context, PartyRoomEntity room)
    {
        var state = StateUtil.JoinStates(
            StatesList.Default,
            StatesList.RoomSelection,
            StatesList.EnterRoomAlias);
        services.Database.Users.SetState(context.User, state);
        context.State.ReplaceState(state.ToString());

        return context.GlobalMessageHandler.Handle(
            new Message()
            {
                Chat = chat,
                Text = MessageParser.GenerateRoomButton(room)
            },
            context.User
        );
    }

    private bool TryGetRoom(string strRoomId, out PartyRoomEntity? room)
    {
        if (Guid.TryParse(strRoomId, out var roomId))
        {
            room = services.Database.Rooms.GetById(roomId);
            return room is { };
        }

        room = default;
        return false;
    }
}
