using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class GlobalState : MessageStateBase
{
    public GlobalState(ServicesContext services, DefaultState defaultState)
        : base(services, StatesList.Global)
    {
        Commands[BotCommandsList.Start] = new StartBotCommand(services, defaultState);
    }

    public override Task<bool> ProcessMessage(Chat chat, MessageContext context)
        => Services.ProcessHandler.HandleCommand(this, chat, context);
}
