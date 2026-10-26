using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.Commands.InRoom;
using SecretSantaTgBot.Models.Commands.InRoom.Admin;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.Common;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class InRoomState : MessageStateBase
{
    public InRoomState(ServicesContext services) : base(services, StatesList.InRoom)
    {
        var roomUpdateState = new ModifyRoomState(services);
        var roomLeaveConfirmationState = new ConfirmationState(
            services,
            StatesList.LeaveConfirmation,
            new LeaveRoomConfirmationBotCommand(services));
        var addWishesState = new WishAddingState(services);
        var notifyEveryoneState = new NotifyEveryoneInRoomState(services);

        States[roomUpdateState.Title] = roomUpdateState;
        States[roomLeaveConfirmationState.Title] = roomLeaveConfirmationState;
        States[addWishesState.Title] = addWishesState;
        States[notifyEveryoneState.Title] = notifyEveryoneState;

        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.ShowRoomInfo] = new ShowRoomBotCommand(services);
        Commands[BotCommandsList.ShowMe] = new ShowMeBotCommand(services);
        Commands[BotCommandsList.ShowMyTarget] = new ShowMyTargetBotCommand(services);
        Commands[BotCommandsList.UpdateRoom] = new UpdateRoomCommand(services, roomUpdateState);
        Commands[BotCommandsList.LeaveRoom] = new LeaveRoomBotCommand(services, roomLeaveConfirmationState);
        Commands[BotCommandsList.StartWishes] = new StartWishesBotCommand(addWishesState);
        Commands[BotCommandsList.ClearWishes] = new ClearWishesBotCommand(services);
        Commands[BotCommandsList.NotifyEveryone] = new NotifyEveryoneCommand(services, notifyEveryoneState);
        Commands[BotCommandsList.StartSanta] = new StartSantaCommand(services);
        Commands[BotCommandsList.Back] = new BackToMenuBotCommand(services);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleStates(this, chat, context) ||
            await Services.ProcessHandler.HandleCommand(this, chat, context);
}
