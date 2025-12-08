using System.Text;

using SecretSantaTgBot.CommandStates;
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Storage;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretSantaTgBot;

public class InRoomCommandState : CommandStateBase
{
    public const string TITLE = "in_room";

    private readonly Dictionary<string, CommandInfo> _commands;
    private readonly Dictionary<string, CommandStateBase> _innerStates;

    private readonly TelegramBotClient _bot;
    private readonly SantaDatabase _db;
    private readonly MessagesDictionary _msgDict;
    private readonly string _lang;
    private readonly MessageBroker _csm;

    public InRoomCommandState(MessageBroker csm)
    {
        _bot = csm.Bot;
        _db = csm.DB;
        _msgDict = csm.MsgDict;
        _lang = csm.Lang;
        _csm = csm;

        _commands = new List<CommandInfo> {
            new("/help", _msgDict[_lang].CommandHelp, CommandHelp),
            new("/show_room_info", _msgDict[_lang].CommandShowRoomInfo, CommandShowRoomInfo),
            new("/leave_room", _msgDict[_lang].CommandLeaveRoom, CommandLeaveRoom)
            {
                Access = AccessRights.NotForAdmin
            },
            new("/show_me", _msgDict[_lang].CommandShowMe, CommandShowMe),
            new("/show_my_target", _msgDict[_lang].CommandShowTarget, CommandShowTarget),
            new("/start_wishes", _msgDict[_lang].CommandStartWishes, CommandStartWishes),
            new("/stop_wishes", _msgDict[_lang].CommandStopWishes, CommandStopWishes),
            new("/clear_wishes", _msgDict[_lang].CommandClearWishes, CommandClearWishes),
            new("/start_santa", _msgDict[_lang].StartSanta, CommandStartSecretSanta)
            {
                Access = AccessRights.Admin
            },
            new("/back", _msgDict[_lang].CommandBack, CommandBack),
        }.ToDictionary(x => x.Command);

        _innerStates = new()
        {
            [InRoomWishesState.TITLE] = new InRoomWishesState(csm)
        };
    }

    public override Task OnMessage(Message msg, UserTg user)
    {
        if (msg.Type != MessageType.Text || !msg.Text!.StartsWith('/'))
        {
            var states = user.CurrentState is not null
                ? NameParser.ParseArgs(user.CurrentState!)
                : [];
            if (states.Length > 1)
            {
                if (_innerStates.TryGetValue(states[1], out var innserState))
                    return innserState.OnMessage(msg, user);
            }

            return OnCommandError(msg.Chat);
        }

        var text = msg.Text!.Trim();
        var words = NameParser.ParseArgs(text);
        var command = words[0];
        var args = words.Length > 1
            ? words[1..]
            : [];

        var cmd = _commands.GetValueOrDefault(command);
        if (cmd is null)
            return OnCommandError(msg.Chat);

        user.CurrentState = default;
        _db.Users.Update(user);

        return cmd.Callback.Invoke(msg.Chat, user, args);
    }



    private Task OnCommandError(Chat chat)
        => _bot.SendMessage(chat, _msgDict[_lang].CommandError);

    private void UpdateUserState(UserTg user, string? state)
    {   
        user.CurrentState = state;
        _db.Users.Update(user);
    }

    private bool IsAdmin(UserTg user)
        => user.SelectedRoom!.Admin.Id == user.Id;

    private Participant GetParticipantById(UserTg user, long userId)
        => user.SelectedRoom!.Users.First(x => x.Id == userId);

    private Participant GetMeAsParticipant(UserTg user)
        => GetParticipantById(user, user.Id);



    private Task CommandHelp(Chat chat, UserTg user, string[] args)
    {
        var msg = MessageBuilder.GetHelpMessage(_msgDict[_lang], _commands.Values, IsAdmin(user));

        return _bot.SendMessage(chat, msg,
                parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
    }

    private Task CommandBack(Chat chat, UserTg user, string[] args)
    {
        user.SelectedRoom = default;
        _db.Users.Update(user);

        return _csm.UpdateAfterStatusChanged(user);
    }

    private async Task CommandLeaveRoom(Chat chat, UserTg user, string[] args)
    {
        if (IsAdmin(user))
        {
            await _bot.SendMessage(chat, _msgDict[_lang].AdminCantLeaveRoom);
            return;
        }

        var room = user.SelectedRoom!;
        var part = GetMeAsParticipant(user);

        user.SelectedRoom = default;
        user.AvailableRooms.Remove(room);
        room.Users.Remove(part);

        _db.Users.Update(user);
        _db.Rooms.Update(room);

        foreach (var u in room.Users)
            await _bot.SendMessage(u.Id, $"{part.RealName} (@{part.Username}) {_msgDict[_lang].UserLeavedRoomForAll}");

        await _bot.SendMessage(chat,
            _msgDict[_lang].UserLeavedRoom,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private Task CommandShowRoomInfo(Chat chat, UserTg user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participants = room.Users;
        var admin = GetParticipantById(user, room.Admin.Id);

        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>\"{room.Title}\"</b>");
        strBldr.AppendLine($"<b>room code: </b><u>{room.Id}</u>");
        strBldr.AppendLine($"admin: {admin.RealName} - @{admin.Username}");
        strBldr.AppendLine($"{room.PartyDescription}");
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{_msgDict[_lang].ParticipantsList} ({participants.Count})</b>");

        foreach (var prt in participants)
            strBldr.AppendLine($" ► {prt.RealName} - @{prt.Username}");

        return _bot.SendMessage(chat,
            strBldr.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }



    private Task CommandShowMe(Chat chat, UserTg user, string[] args)
    {
        var target = GetMeAsParticipant(user);
        return ShowUserInfo(user.Id, target, 
            _msgDict[_lang].UserWishesList, 
            _msgDict[_lang].UserHaveZeroWishes);
    }

    private Task CommandShowTarget(Chat chat, UserTg user, string[] args)
    {
        var me = GetMeAsParticipant(user);
        if (me.TargetUserId is null)
            return _bot.SendMessage(chat, _msgDict[_lang].SecretSantaStillOffline);

        var target = GetParticipantById(user, me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            _msgDict[_lang].TargetWishesList,
            _msgDict[_lang].TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, Participant target, string headerMsg, string emptyMsg)
    {
        var wishes = target.Wishes;

        var strBldr = new StringBuilder();
        strBldr.AppendLine($"<b>{target.RealName} - @{target.Username}</b>");

        if (target.Wishes.Count == 0)
        {
            await _bot.SendMessage(chatId,
                strBldr.ToString(),
                parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
            return;
        }
        
        strBldr.AppendLine();
        strBldr.AppendLine($"<b>{headerMsg}</b>");

        foreach (var wish in target.Wishes.Where(x => x.Images is not { Count: > 0 }))
            strBldr.AppendLine($" ► {wish.Message}");

        await _bot.SendMessage(chatId,
            strBldr.ToString(),
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());

        foreach (var wish in wishes.Where(x => x.Images is { Count: > 0 }))
        {
            var images = wish.Images!
                .Select(x => new InputMediaPhoto(x))
                .ToArray();
            images.First().Caption = wish.Message;
            await _bot.SendMediaGroup(chatId, images);
        }
    }



    private Task CommandStartWishes(Chat chat, UserTg user, string[] args)
    {
        var states = NameParser.JoinArgs(TITLE, InRoomWishesState.TITLE);
        UpdateUserState(user, states);

        return _bot.SendMessage(chat,
            _msgDict[_lang].UserStartWishes,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task CommandStopWishes(Chat chat, UserTg user, string[] args)
    {
        UpdateUserState(user, default);

        await _bot.SendMessage(chat,
            _msgDict[_lang].UserStopWishes,
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
        await _csm.UpdateAfterStatusChanged(user);
    }

    private Task CommandClearWishes(Chat chat, UserTg user, string[] args)
    {
        var me = GetMeAsParticipant(user);
        me.Wishes.Clear();
        _db.Rooms.Update(user.SelectedRoom!);

        return _bot.SendMessage(chat, _msgDict[_lang].UserWishesCleared);
    }



    private async Task CommandStartSecretSanta(Chat chat, UserTg user, string[] args)
    {
        var room = user.SelectedRoom!;
        if (room.Admin.Id != user.Id)
        {
            await _bot.SendMessage(chat, _msgDict[_lang].NeedAdminRights);
            return;
        }

        var random = new Random();
        var participants = room.Users;
        
        if (participants.Count < 2)
        {
            await _bot.SendMessage(chat, _msgDict[_lang].NotEnoughParticipants);
            return;
        }

        var ap = participants.ToList();
        

        for (int i = 0; i < participants.Count; ++i)
        {
            var fap = ap.Where(x => x != participants[i]).ToArray();

            if (fap.Length == 0)
            {
                participants[i].TargetUserId = participants[i - 1].TargetUserId;
                participants[i - 1].TargetUserId = ap.Last().Id;
                ap.Remove(ap.Last());
                continue;
            }

            var idx = random.Next(0, fap.Length);
            participants[i].TargetUserId = fap[idx].Id;

            ap.Remove(fap[idx]);
        }

        _db.Rooms.Update(room);

        foreach (var part in participants)
        {
            var target = participants.First(x => x.Id == part.TargetUserId);

            var strBldr = new StringBuilder();
            strBldr.AppendLine($"{_msgDict[_lang].RoomNumber} \"{room.Title}\" - {room.Id}");
            strBldr.AppendLine(_msgDict[_lang].SantaFinished);
            strBldr.AppendLine($"{_msgDict[_lang].UserTarget} {target.RealName} @{target.Username}");

            await _bot.SendMessage(part.Id,
                strBldr.ToString(),
                parseMode: ParseMode.Html);
            }
    }
}
