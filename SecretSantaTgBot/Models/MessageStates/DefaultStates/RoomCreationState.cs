using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomCreationState : MessageStateBase
{
    public RoomCreationState(ServicesContext services)
        : base(services, StatesList.RoomCreation)
    {
        var enterNameState = new EnterValueState(
            services,
            StatesList.EnterInRoomName,
            StateArgsList.Name,
            BotContent.Messages.EnterName,
            Registration,
            context => MessageParser.GetCommandsAsButtons(this)!);
        var enterDescriptionState = new EnterValueState(
            services,
            StatesList.EnterDescription,
            StateArgsList.Description,
            BotContent.Messages.EnterDescription,
            StateUtil.NextStateCallback(enterNameState),
            context => MessageParser.GetCommandsAsButtons(this)!);
        var enterTitleState = new EnterValueState(
            services,
            StatesList.EnterTitle,
            StateArgsList.Title,
            BotContent.Messages.EnterTitle,
            StateUtil.NextStateCallback(enterDescriptionState),
            context => MessageParser.GetCommandsAsButtons(this)!);

        States[enterTitleState.Title] = enterTitleState;
        States[enterDescriptionState.Title] = enterDescriptionState;
        States[enterNameState.Title] = enterNameState;

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterTitle].PrepareState(context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.ProcessHandler.HandleCommand(this, chat, context);

    private Task Registration(Chat chat, MessageContext context)
    {
        var user = context.User;
        var title = context.State.StateObject.Arguments[StateArgsList.Title];
        var description = context.State.StateObject.Arguments[StateArgsList.Description];
        var adminName = context.State.StateObject.Arguments[StateArgsList.Name];

        var room = Services.Database.CreateRoom(user, title, description, adminName);
        FinishState(user);

        var message = Services.MessageBuilder.BuildCreateRoom(room.Id.ToString());
        return Services.Notifications.SendMessage(user.TelegramUserId, message);
    }
}
