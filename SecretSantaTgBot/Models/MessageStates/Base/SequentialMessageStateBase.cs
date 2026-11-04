using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class SequentialMessageStateBase(
    string title,
    MessageStateBase? parrent,
    ServicesContext services)
    :MessageStateBase(title, parrent, services)
{
    protected abstract MessageStateBase FirstState { get; }

    public sealed override Task PrepareState(MessageContext context)
        => FirstState.PrepareState(context);

    public sealed override Task SendWelcomeMessage(Chat chat, MessageContext context)
        => FirstState.SendWelcomeMessage(chat, context);

}
