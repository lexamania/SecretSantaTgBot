using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class InRoomRegistrationState : MessageStateBase
{
    public InRoomRegistrationState(ServicesContext services)
        : base(services, StatesList.RoomRegistration)
    {
        States[StatesList.EnterInRoomName] = new EnterValueState(
            services,
            StatesList.EnterInRoomName,
            StateArgsList.Name,
            context => context.Localization.EnterRealName,
            Registration,
            context => MessageParser.GetCommandsAsButtons(this)!);

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterInRoomName].PrepareState(context);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleState(this, chat, context)
        || await Services.ProcessHandler.HandleCommand(this, chat, context);
    
    private Task Registration(Chat chat, MessageContext context)
    {
        var user = context.User;
        var roomId = context.State.StateObject.Arguments[StateArgsList.RoomId];
        var userName = context.State.StateObject.Arguments[StateArgsList.Name];

        if (!TryGetRoom(roomId, out var room))
        {
            FinishState(user);
            return Services.Notifications.SendErrorMessage(
                chat.Id,
                context.Localization.RoomDoesntExist);
        }

        Services.Database.AddUserToRoom(context.User, room!, userName);
        FinishState(context.User);

        return Task.WhenAll(
            NotifyUser(chat.Id, context),
            Services.Notifications.SendMessage(
                chat.Id,
                context.Localization.UserNewParticipation)
        );
    }
        
    private async Task NotifyUser(long chatId, MessageContext context)
    {
        await Services.Notifications.SendMessage(
                chatId,
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
