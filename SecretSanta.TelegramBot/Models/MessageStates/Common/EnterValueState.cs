using SecretSanta.TelegramBot.Enums;
using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.Shared.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.Common;

public class EnterValueState : MessageStateBase
{
    public delegate Task ProcessHandler(Chat chat, MessageContext context);
    public delegate string[] ButtonsGenerator(MessageContext context);

    public EnterValueState(
        IMessageState parrent,
        ServicesContext services,
        string title,
        string valueKey,
        string messageKey,
        ProcessHandler handler,
        ButtonsGenerator? buttons = default)
        : base(title, parrent, services)
    {
        _valueKey = valueKey;
        _messageKey = messageKey;
        _postProcess = handler;
        _buttons = buttons;

        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    private readonly string _valueKey;
    private readonly string _messageKey;
    private readonly ProcessHandler _postProcess;
    private readonly ButtonsGenerator? _buttons;

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
        => Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            _messageKey,
            GetButtons(context));

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.CustomHandler.HandleCommand(this, chat, context))
            return true;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendWrongText(chat.Id);
            return true;
        }

        context.State.Arguments[_valueKey] = context.Message.TextMsg!.Text;
        Services.Database.Users.SetState(context.User, context.State);

        await _postProcess.Invoke(chat, context);
        return true;
    }

    private string[] GetButtons(MessageContext context)
    {
        var cmdBtns = ButtonUtil.GetCommandsAsButtons(this);
        var btns = _buttons?.Invoke(context) ?? [];
        return _buttons is null
            ? cmdBtns
            : [..btns, ..cmdBtns];
    }
}
