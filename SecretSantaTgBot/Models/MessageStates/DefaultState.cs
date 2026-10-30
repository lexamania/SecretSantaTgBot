using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Default;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class DefaultState : MessageStateBase
{
    public DefaultState(ServicesContext services) : base(services, StatesList.Default)
    {
        var selectRoomState = new RoomSelectionState(services);
        var deleteRoomState = new RoomDeletingState(services);
        var createRoomState = new RoomCreationState(services);

        States[createRoomState.Title] = createRoomState;
        States[selectRoomState.Title] = selectRoomState;
        States[deleteRoomState.Title] = deleteRoomState;

        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.SelectRoom] = new SelectRoomBotCommand(services, selectRoomState);
        Commands[BotCommandsList.CreateRoom] = new CreateRoomBotCommand(createRoomState);
        Commands[BotCommandsList.DeleteRoom] = new DeleteRoomBotCommand(services, deleteRoomState);
        Commands[BotCommandsList.ShowRooms] = new ShowRoomsBotCommand(services);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleState(this, chat, context) ||
            await Services.ProcessHandler.HandleCommand(this, chat, context);
}
