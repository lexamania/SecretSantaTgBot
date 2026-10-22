using SecretSantaTgBot.Attributes;
using SecretSantaTgBot.Models;

namespace SecretSantaTgBot.Services.Commands;

[RequiredService(InjectionType.Singleton)]
public class PopularCommandsBuilder(
    NotificationService notifications,
    MessageBuilder messageBuilder)
{
    // TODO
    public CommandModel BuildHelpCommand
    protected Task CommandHelp(Chat chat, UserEntity user, string[] args)
    {
        var isAdmin = user.SelectedRoom?.Admin.Id == user.Id;
        var msg = messageBuilder.BuildHelpMessage(Commands.Values, isAdmin);
        return Notifications.SendMessage(chat.Id, msg);
    }
}
