using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class GlobalState : MessageStateBase
{
    public GlobalState(ServicesContext services) : base([StatesList.Global])
    {
        Commands[BotCommandsList.Start] = new StartBotCommand(services);
    }

    public override async Task<bool> ProcessMessage(Chat chat, BotCommandContext context)
    {
        if (context.Message.Type != MessageType.Command)
            return false;
        
        if (!Commands.TryGetValue(context.Message.CommandMsg!.Command, out var cmd))
            return false;

        await cmd.Execute(chat, context);
        return true;
    }
}
