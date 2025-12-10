using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.CommandStates;

public class InRoomNameRegistrationState(MessageBrokerService csm, string parentTitle)
    : MessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "registration";

    protected override string Message => Msgs.EnterRealName;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        if (!MessageParser.IsMessage(msg, out var message))
        {
            await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Msgs.EnterRealName);
            return true;
        }

        var room = user.SelectedRoom!;
        var participant = room.Users.First(x => x.Id == user.Id);
        participant.RealName = message;

        DB.Rooms.Update(room);
        UpdateUserState(user, default);

        await NotifyService.SendMessage(msg.Chat.Id, Msgs.UserParticipationEnd);
        await Csm.UpdateAfterStatusChanged(user);
        return true;
    }

    public override Task StartState(UserTg user, string[] args)
    {
        var strArgs = NameParser.JoinArgs(args);
        var state = NameParser.JoinArgs(Title, strArgs);
        UpdateUserState(user, state);

        return NotifyService.SendMessage(user.Id, Message);
    }
}
