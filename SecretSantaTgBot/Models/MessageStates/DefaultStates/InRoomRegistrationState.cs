using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class InRoomRegistrationState : InternalMessageStateBase
{
    public InRoomRegistrationState(ServicesContext services)
        : base(services, StatesList.RoomRegistration)
    {
        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.EnterRealName,
            MessageParser.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendErrorCommandMessage(
                chat.Id,
                context.Localization.EnterRealName);
            return true;
        }

        if (!context.State.StateObject.Arguments.TryGetValue(StateArgsList.RoomId, out var strRoomId)
            || !TryGetRoom(strRoomId, out var room))
        {
            FinishState(context.User);
            await Services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.RoomDoesntExist);
            return true;
        }

        Services.Database.AddUserToRoom(context.User, room!);

        var participant = context.User.GetAsParticipant(room!)!;
        participant.RealName = context.Message.TextMsg!.Text;

        Services.Database.Rooms.Update(room!);
        FinishState(context.User);

        await Task.WhenAll(
            NotifyUser(chat, context),
            Services.Notifications.SendMessage(
                chat.Id,
                context.Localization.UserNewParticipation)
        );
        return true;
    }

    private async Task NotifyUser(Chat chat, MessageContext context)
    {
        await Services.Notifications.SendMessage(
                chat.Id,
                context.Localization.UserParticipationEnd);
        await context.GlobalMessageHandler.SendHelpMenu(context.User);
    }
    

    private bool TryGetRoom(string strRoomId, out PartyRoomEntity? room)
    {
        if (Guid.TryParse(strRoomId, out var roomId))
        {
            room = Services.Database.Rooms.GetById(roomId);
            return room is { };
        }

        room = default;
        return false;
    }
}
