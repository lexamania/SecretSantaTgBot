using SecretSanta.TelegramBot.Models.Commands.Base;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.Base;

public interface IMessageState
{
    public string Title { get; }
    public IMessageState? Parrent { get; }
    public IReadOnlyDictionary<string, IBotCommand> ReadOnlyCommands { get; }
    public IReadOnlyDictionary<string, IMessageState> ReadOnlyStates { get; }

    public Task PrepareState(MessageContext context);
    public Task SendWelcomeMessage(Chat chat, MessageContext context);
    public Task StartState(Chat chat, MessageContext context);
    public Task<bool> ProcessMessage(Chat chat, MessageContext context);
}
