using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Default;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class DefaultState : MessageStateBase
{
    public DefaultState(ServicesContext services)
        : base(StatesList.Default, null, services)
    {
        var selectRoomState = new RoomSelectionState(this, services);
        var deleteRoomState = new RoomDeletingState(this, services);
        var createRoomState = new RoomCreationState(this, services);
        var InRoomRegistrationState = new InRoomRegistrationState(this, services);

        States[createRoomState.Title] = createRoomState;
        States[selectRoomState.Title] = selectRoomState;
        States[deleteRoomState.Title] = deleteRoomState;
        States[InRoomRegistrationState.Title] = InRoomRegistrationState;

        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.ShowRooms] = new ShowRoomsBotCommand(services);
        Commands[BotCommandsList.CreateRoom] = new CreateRoomBotCommand(createRoomState);
        Commands[BotCommandsList.DeleteRoom] = new DeleteRoomBotCommand(services, deleteRoomState);
        // Commands[BotCommandsList.SelectRoom] = new SelectRoomBotCommand(services, selectRoomState);
    }

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
        => Commands[BotCommandsList.Help].Execute(chat, context);

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.CustomHandler.HandleCommand(this, chat, context);
}
