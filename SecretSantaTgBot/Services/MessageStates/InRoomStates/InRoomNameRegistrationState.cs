using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomNameRegistrationState(ServiceContainer container, string parentTitle)
    : MessageStateBase(container, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "registration";
    private string Message => Msgs.EnterRealName;

    public override Task PrepareState(UserEntity user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        UpdateUserState(user, state);

        return Notification.SendMessage(user.Id, Message);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
        {
            await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;
        participant.RealName = message;

        Database.RoomDirectory.Update(room);
        UpdateUserState(user, default);

        await Notification.SendMessage(msg.Chat.Id, Msgs.UserParticipationEnd);
        await MessageBroker.SendHelpMenu(user);
        return true;
    }
}
