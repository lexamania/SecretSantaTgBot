using SecretSantaTgBot.Storage.Entities;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.Commands;

public class HelpCommand (TelegramBotService telegramBot)
{

    public Task SendHelpMenu(UserEntity user)
    {
        var msg = new Message()
        {
            Text = "/help",
            Chat = new()
            {
                Id = user.Id,
                Username = user.Username
            }
        };

        return telegramBot.Bot. CallMessage(msg, user);
    }
}
