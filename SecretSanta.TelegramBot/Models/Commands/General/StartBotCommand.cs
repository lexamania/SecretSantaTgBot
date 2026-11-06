using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.TelegramBot.Models.Commands.Base;
using SecretSanta.TelegramBot.Models.MessageStates;
using SecretSanta.Shared.Storage.Entities;
using SecretSanta.Shared.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.Commands.General;

public class StartBotCommand(
    ServicesContext services,
    DefaultState defaultState)
    : BotCommandBase(BotCommandsList.Start)
{
    public override Task Execute(Chat chat, MessageContext context)
    {
        var args = context.Message.CommandMsg!.Args;

        if (args.Length == 0)
        { 
            return services.GlobalHandler.ExitState(chat, context.User);
        }

        var room = services.Database.Rooms.GetById(args[0]);
        if (room is null)
        {
            return services.Notifications.SendStagedMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
        }

        return context.User.GetAvailableRoom(room.Id) is null
            ? RegisterInRoom(chat, context, room)
            : RedirectToRoom(chat, context, room);
    }

    private Task RegisterInRoom(
        Chat chat,
        MessageContext context,
        PartyRoomEntity room)
    {
        context.State.Arguments[StateArgsList.RoomId] =
            room.Id.ToString();
        return defaultState
            .ReadOnlyStates[StatesList.InRoomRegistration]
            .StartState(chat, context);
    }

    private Task RedirectToRoom(
        Chat chat,
        MessageContext context,
        PartyRoomEntity room)
    {
        context.State.Arguments[StateArgsList.RoomAlias] =
            ButtonUtil.GenerateRoomButton(room);
        return defaultState
            .ReadOnlyStates[StatesList.RoomSelection]
            .ProcessMessage(chat, context);
    }
}
