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
            context => context.Localization.EnterRealName,
            Registration,
            context => MessageParser.GetCommandsAsButtons(this)!);
        var enterDescriptionState = new EnterValueState(
            services,
            StatesList.EnterDescription,
            StateArgsList.Description,
            context => context.Localization.RoomCreationEnterDescription,
            StateUtil.NextStateCallback(enterNameState),
            context => MessageParser.GetCommandsAsButtons(this)!);
        var enterTitleState = new EnterValueState(
            services,
            StatesList.EnterTitle,
            StateArgsList.Title,
            context => context.Localization.RoomCreationEnterTitle,
            StateUtil.NextStateCallback(enterDescriptionState),
            context => MessageParser.GetCommandsAsButtons(this)!);

        States[enterTitleState.Title] = enterTitleState;
        States[enterDescriptionState.Title] = enterDescriptionState;
        States[enterNameState.Title] = enterNameState;

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => States[StatesList.EnterTitle].PrepareState(context);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleState(this, chat, context)
        || await Services.ProcessHandler.HandleCommand(this, chat, context);

    private Task Registration(Chat chat, MessageContext context)
    {
        var user = context.User;
        var title = context.State.StateObject.Arguments[StateArgsList.Title];
        var description = context.State.StateObject.Arguments[StateArgsList.Description];
        var adminName = context.State.StateObject.Arguments[StateArgsList.Name];

        var room = Services.Database.CreateRoom(user, title, description, adminName);
        FinishState(user);

        var message = Services.MessageBuilder.BuildCreateRoomMessage(room.Id.ToString());
        return Services.Notifications.SendMessage(user.TelegramUserId, message);
    }
}
