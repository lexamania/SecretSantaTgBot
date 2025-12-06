using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot;

public class MessageBroker(TelegramBotClient bot)
{
    private readonly TelegramBotClient _bot = bot;

    public async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is not { } text)
        {
            Console.WriteLine($"Received a message of type {msg.Type}");
            return;
        }

        if (!text.StartsWith('/'))
        {
            await OnTextMessage(msg);
            return;
        }

        var words = text.Split(' ').Where(x => x.Length > 0).ToArray();
        var command = words[0];
        var args = words.Length > 1
            ? words[1..]
            : [];

        await OnCommand(command, args, msg);
    }

    private async Task OnTextMessage(Message msg)
    {
        Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
        await OnCommand("/start", [""], msg);
    }

    private async Task OnCommand(string command, string[] args, Message msg)
    {
        Console.WriteLine($"Received command: {command} {args}");
        // switch (command)
        // {
        //     case "/start":
        //         await _bot.SendMessage(msg.Chat, """
        //         <b><u>Bot menu</u></b>:
        //         /photo [url]    - send a photo <i>(optionally from an <a href="https://picsum.photos/310/200.jpg">url</a>)</i>
        //         /inline_buttons - send inline buttons
        //         /keyboard       - send keyboard buttons
        //         /remove         - remove keyboard buttons
        //         /poll           - send a poll
        //         /reaction       - send a reaction
        //         """, parseMode: ParseMode.Html, linkPreviewOptions: true,
        //             replyMarkup: new ReplyKeyboardRemove()); // also remove keyboard to clean-up things
        //         break;
        //     case "/photo":
        //         if (args.StartsWith("http"))
        //             await _bot.SendPhoto(msg.Chat, args, caption: "Source: " + args);
        //         else
        //         {
        //             await _bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        //             await Task.Delay(2000); // simulate a long task
        //             await using var fileStream = new FileStream("_bot.gif", FileMode.Open, FileAccess.Read);
        //             await _bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
        //         }
        //         break;
        //     case "/inline_buttons":
        //         await _bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: new InlineKeyboardButton[][] {
        //         ["1.1", "1.2", "1.3"],
        //         [("WithCallbackData", "CallbackData"), ("WithUrl", "https://github.com/TelegramBots/Telegram.Bot")]
        //     });
        //         break;
        //     case "/keyboard":
        //         await _bot.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: new[] { "MENU", "INFO", "LANGUAGE" });
        //         break;
        //     case "/remove":
        //         await _bot.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
        //         break;
        //     case "/poll":
        //         await _bot.SendPoll(msg.Chat, "Question", ["Option 0", "Option 1", "Option 2"], isAnonymous: false, allowsMultipleAnswers: true);
        //         break;
        //     case "/reaction":
        //         await _bot.SetMessageReaction(msg.Chat, msg.Id, ["❤"], false);
        //         break;
        // }
    }

    private Task CommandStart(Chat chat)
        => CommandHelp(chat);

    private Task CommandHelp(Chat chat)
        => _bot.SendMessage(chat, """
                <b><u>Bot menu</u></b>:
                /help              - сторінка з усіма командами
                /participate       - взяти участь в Таємному Санті
                /stop_participate  - прибрати себе зі списків Таємного Санти
                /show_participants - показати список учасників
                /show_my_target    - показати мою ціль на Таємного Санту
                /start_wishes      - розпочати додавати бажання
                /stop_wishes       - зупинити додавати бажання
                /clear_wishes      - очистити список своїх бажань
                /show_my_wishes    - показати мої бажання
                """, 
                parseMode: ParseMode.Html, linkPreviewOptions: true,
                replyMarkup: new ReplyKeyboardRemove());

    private Task CommandParticipate(Chat chat)
    {

    }

    private Task CommandStopParticipate(Chat chat)
    {

    }

    private Task CommandStartWishes(Chat chat)
    {

    }

    private Task CommandStopWishes(Chat chat)
    {

    }

    private Task CommandClearWishes(Chat chat)
    {

    }

    private Task CommandShowSantaTarget(Chat chat)
    {

    }

    private Task CommandShowMyWishes(Chat chat)
    {

    }

    private Task CommandShowParticipationList(Chat chat)
    {

    }
}
