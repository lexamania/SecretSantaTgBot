using SecretSantaTgBot.Models.Commands.Base;
using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.Commands.General;

public class StartBotCommand(ServicesContext services)
    : BotCommandBase(BotCommandsList.Start)
{
    public override async Task Execute(Chat chat, BotCommandContext context)
    {
        var args = context.Message.CommandMsg!.Args;
        var user = context.User;

        if (args.Length == 0)
        {
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

        if (!user.AvailableRooms.Any(x => x.Id == room!.Id))
        {
            services.Database.AddUserToRoom(user, room!);
            await services.Notifications.SendMessage(
                chat.Id,
                context.Localization.UserNewParticipation);
        }

        await RedirectToRoom(chat, context, room!);
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

    private Task RedirectToRoom(Chat chat, BotCommandContext context, PartyRoomEntity room)
    {
        var state = StateUtil.JoinStates(StatesList.Default, StatesList.RoomSelection);
        services.Database.Users.UpdateWithState(context.User, state);

        return context.GlobalMessageHandler.Handle(
            new Message()
            {
                Chat = chat,
                Text = $"{room.Title} {room.Id}"
            },
            context.User
        );
    }
}
