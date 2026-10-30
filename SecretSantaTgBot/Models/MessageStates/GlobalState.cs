using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class GlobalState : MessageStateBase
{
    public GlobalState(ServicesContext services)
        : base(services, StatesList.Global)
    {
        var defaultState = new DefaultState(services);

        States[StatesList.Default] = defaultState;
        States[StatesList.InRoom] = new InRoomState(services);

        Commands[BotCommandsList.Start] = new StartBotCommand(services, defaultState);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => await Services.ProcessHandler.HandleCommand(this, chat, context)
        || await Services.ProcessHandler.HandleGlobalState(this, chat, context);
}
