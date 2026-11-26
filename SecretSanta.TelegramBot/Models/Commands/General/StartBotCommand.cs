using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.Shared.Extensions;
using SecretSanta.Shared.Models;
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
    public override async Task Execute(Chat chat, MessageContext context)
    {
        var args = context.Message.CommandMsg!.Args;

        if (args.Length == 0)
        { 
            await services.GlobalHandler.ResetStateAsync(chat, context.User.TelegramUserId);
            return;
        }

        if (!RoomId.TryParse(args[0], out var roomId))
        {
            await services.Notifications.SendStagedMessage(
                chat.Id, services.Localizer.Messages.ErrRoomDoesntExist);
            return;
        }

        var room = await services.Database.Rooms.GetByIdAsync(roomId);
        if (room is null)
        {
            await services.Notifications.SendStagedMessage(
                chat.Id, services.Localizer.Messages.ErrRoomDoesntExist);
            return;
        }

        if (context.User.GetAvailableRoom(room.Id) is null)
            await RegisterInRoom(chat, context, room);
        else
            await RedirectToRoom(chat, context, room);
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
