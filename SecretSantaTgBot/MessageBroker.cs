using SecretSantaTgBot.CommandStates;
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaTgBot;

public class MessageBroker
{
    private readonly Dictionary<string, CommandStateBase> _states;

    public TelegramBotClient Bot { get; }
    public SantaDatabase DB { get; }
    public MessagesDictionary MsgDict { get; }
    public string Lang { get; }

    public MessageBroker(TelegramBotClient bot, SantaDatabase db, MessagesDictionary msgDict)
    {
        Bot = bot;
        DB = db;
        MsgDict = msgDict;
        Lang = "UA";

        _states = new()
        {
            [DefaultState.TITLE] = new DefaultState(this),
            [RoomCreateState.TITLE] = new RoomCreateState(this),
            [RegistrationState.TITLE] = new RegistrationState(this),

            [InRoomCommandState.TITLE] = new InRoomCommandState(this),
        };
    }

    public async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Type != MessageType.Text && msg.Type != MessageType.Photo)
        {
            Console.WriteLine($"Received a message of type {msg.Type}");
            await Bot.SendMessage(msg.Chat, MsgDict[Lang].CommandError);
            return;
        }

        Console.WriteLine($"Received a message: \"{msg.Text}\" in {msg.Chat} from {msg.Chat.Username}");
        
        var user = CreateUserIfNeed(msg.Chat);
        await CallMessage(msg, user);
    }

    public Task UpdateAfterStatusChanged(UserTg user)
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

        return CallMessage(msg, user);
    }

    private async Task CallMessage(Message msg, UserTg user)
    {
        var stateStr = GetCurrentState(user);

        if (!_states.TryGetValue(stateStr, out var state))
        {
            await Bot.SendMessage(msg.Chat, MsgDict[Lang].CommandError);
            return;
        }
        
        try
        {
            await state.OnMessage(msg, user);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private string GetCurrentState(UserTg user)
    {
        if (user.CurrentState is not null)
            return NameParser.ParseArgs(user.CurrentState)[0];

        return user.SelectedRoom != null
            ? InRoomCommandState.TITLE
            : DefaultState.TITLE;
    }

    private UserTg CreateUserIfNeed(Chat chat)
    {
        var user = DB.Users
            .Include(x => x.AvailableRooms)
            .Include(x => x.SelectedRoom)
            .FindById(chat.Id);

        if (user is null)
        {
            user = new() { Id = chat.Id, Username = chat.Username!, AvailableRooms = [] };
            DB.Users.Insert(user);
        }
        else if (user.Username != chat.Username)
        {
            user.Username = chat.Username!;
            DB.Users.Update(user);
        }

        return user;
    }
}
