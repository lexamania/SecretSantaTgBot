using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.Commands.InRoom;
using SecretSanta.TelegramBot.Models.Commands.InRoom.Admin;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates;

public class InRoomState : MessageStateBase
{
    public InRoomState(ServicesContext services)
        : base(StatesList.InRoom, null, services)
    {
        var roomUpdateState = new ModifyRoomState(this, services);
        var roomLeaveState = new LeaveRoomState(this, services);
        var addWishesState = new WishAddingState(this, services);
        var notifyEveryoneState = new NotifyEveryoneInRoomState(this, services);

        States[roomUpdateState.Title] = roomUpdateState;
        States[roomLeaveState.Title] = roomLeaveState;
        States[addWishesState.Title] = addWishesState;
        States[notifyEveryoneState.Title] = notifyEveryoneState;

        Commands[BotCommandsList.Help] =new HelpBotCommand(services, this);
        Commands[BotCommandsList.ShowRoomInfo] =new ShowRoomBotCommand(services);
        Commands[BotCommandsList.ShowMe] =new ShowMeBotCommand(services);
        Commands[BotCommandsList.ShowMyTarget] =new ShowMyTargetBotCommand(services);
        Commands[BotCommandsList.ModifyRoom] =new UpdateRoomCommand(services, roomUpdateState);
        Commands[BotCommandsList.LeaveRoom] =new LeaveRoomBotCommand(services, roomLeaveState);
        Commands[BotCommandsList.AddWishes] =new StartWishesBotCommand(addWishesState);
        Commands[BotCommandsList.ClearWishes] =new ClearWishesBotCommand(services);
        Commands[BotCommandsList.NotifyEveryone] =new NotifyEveryoneCommand(services, notifyEveryoneState);
        Commands[BotCommandsList.StartSanta] =new StartSantaCommand(services);
        Commands[BotCommandsList.Back] =new BackToMenuBotCommand(services);
    }

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
        => Commands[BotCommandsList.Help].Execute(chat, context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.CustomHandler.HandleCommand(this, chat, context);
}
