using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.Default;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class DefaultState : MessageStateBase
{
    public DefaultState(ServicesContext services) : base([StatesList.Default])
    {
        _services = services;

        var selectRoomState = new RoomSelectState(services, StateUrl);
        var deleteRoomState = new RoomDeleteState(services, StateUrl);
        var createRoomState = new RoomCreateState(services, StateUrl);

        States[StatesList.RoomCreation] = createRoomState;
        States[StatesList.RoomSelection] = selectRoomState;
        States[StatesList.RoomDeleting] = deleteRoomState;

        Commands[BotCommandsList.SelectRoom] = new SelectRoomBotCommand(services, selectRoomState);
        Commands[BotCommandsList.CreateRoom] = new CreateRoomBotCommand(createRoomState);
        Commands[BotCommandsList.DeleteRoom] = new DeleteRoomBotCommand(services, deleteRoomState);
        Commands[BotCommandsList.ShowRooms] = new ShowRoomsBotCommand(services);
    }

    private readonly ServicesContext _services;

    public override async Task<bool> ProcessMessage(Chat chat, BotCommandContext context)
    {
        if (await _services.ProcessHandler.HandleStates(this, chat, context))
            return true;

        if (context.Message.Type == MessageType.Command &&
            await _services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        return false;
    }
}
