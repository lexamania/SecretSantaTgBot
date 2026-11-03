using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
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
            BotContent.Messages.EnterName,
            Registration,
            context => MessageParser.GetCommandsAsButtons(this)!);

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterInRoomName].PrepareState(context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.ProcessHandler.HandleCommand(this, chat, context);
    
    private async Task Registration(Chat chat, MessageContext context)
    {
        var roomId = context.State.StateObject.Arguments[StateArgsList.RoomId];
        var userName = context.State.StateObject.Arguments[StateArgsList.Name];

        if (!TryGetRoom(roomId, out var room))
        {
            FinishState(context.User);
            await Services.Notifications.SendMessageByKey(
                chat.Id, BotContent.Messages.ErrRoomDoesntExist);
            return;
        }

        Services.Database.AddUserToRoom(context.User, room!, userName);
        Services.Database.Users.EnterRoom(context.User, room!);

        var participant = context.User.GetAsParticipant(room!)!;

        await Services.Notifications.SendMessageByKey(
            chat.Id, BotContent.Messages.UserRegistered);
        await Services.GlobalMessageHandler.SendHelpMenu(context.User);

        await Services.Notifications.NotifyEveryoneInRoom(room!,
                Services.MessageBuilder.BuildNewParticipant(room!, participant),
                [participant.TelegramUserId]);
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
