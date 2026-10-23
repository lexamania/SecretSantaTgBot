using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class InRoomNameRegistrationState(ServicesContext context, string parentTitle)
    : MessageStateBase(context, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "registration";
    private string Message => Localization.EnterRealName;

    public override Task PrepareState(UserEntity user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(StateUrl, strArgs);
        UpdateUserState(user, state);

        return Notifications.SendMessage(user.Id, Message);
    }

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        if (!MessageParser.TryParseText(msg, out var message))
        {
            await Notifications.SendErrorCommandMessage(msg.Chat.Id, Message);
            return true;
        }

        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;
        participant.RealName = message;

        Database.RoomDirectory.Update(room);
        ClearUserState(user);

        await Notifications.SendMessage(msg.Chat.Id, Localization.UserParticipationEnd);
        await MessageHandler.SendHelpMenu(user);
        return true;
    }
}
