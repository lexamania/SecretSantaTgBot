using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Models.MessageStates.DefaultStates;
using SecretSantaTgBot.Models.MessageStates.InRoomStates;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates;

public class InRoomCommandState : MessageStateBase
{
    private const string LEAVE_TITLE = "leave_confirmation";

    private readonly Dictionary<string, MessageStateBase> _innerStates;

    public InRoomCommandState(ServicesContext context) : base([StatesList.InRoom])
    {
        var roomUpdateState = new InRoomUpdateState(context, StateUrl);
        var roomLeaveState = new ConfirmationState(context, StateUrl, LEAVE_TITLE, CommandLeaveRoomConfirmation);
        var wishesState = new InRoomWishesState(context, StateUrl);
        var notifyEveryoneState = new InRoomNotifyEveryoneState(context, StateUrl);
        
        States[StatesList.RoomUpdating] = roomUpdateState; 
        States[StatesList.RoomLeaving] = roomLeaveState; 
        States[StatesList.Wishes] = wishesState; 
        States[StatesList.NotifyEveryone] = notifyEveryoneState; 

        var commands = new List<CommandModel> {
            new("/show_room_info", Localization.CommandShowRoomInfo, CommandShowRoomInfo),
            new("/leave_room", Localization.CommandLeaveRoom, CommandLeaveRoom)
            {
                Access = AccessRights.NotForAdmin
            },
            new("/show_me", Localization.CommandShowMe, CommandShowMe),
            new("/show_my_target", Localization.CommandShowTarget, CommandShowTarget),
            new("/start_wishes", Localization.CommandStartWishes, CommandStartWishes),
            new("/clear_wishes", Localization.CommandClearWishes, CommandClearWishes),
            new("/update_room_info", Localization.CommandUpdateRoom, CommandUpdateRoom)
            {
                Access = AccessRights.Admin
            },
            new("/notify_everyone", Localization.CommandNotifyEveryone, CommandNotifyEveryone)
            {
                Access = AccessRights.Admin
            },
            new("/start_santa", Localization.CommandStartSanta, CommandStartSecretSanta)
            {
                Access = AccessRights.Admin
            },
            new("/back", Localization.CommandBack, CommandBack),
        };

        foreach (var command in commands)
            Commands.Add(command.Command, command);

    }

    private readonly ServicesContext _services;

    public override async Task<bool> ProcessMessage(Chat chat, BotCommandContext context)
    {
        if (await _services.ProcessHandler.HandleStates(this, chat, context))
            return true;

        if (context.Message.Type == MessageType.Command &&
            await _services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        return false;
    }



    private Task CommandBack(Chat chat, UserEntity user, string[] args)
    {
        Database.UserDirectory.UpdateWithClearRoom(user);
        return MessageHandler.SendHelpMenu(user);
    }

    private Task CommandShowRoomInfo(Chat chat, UserEntity user, string[] args)
    {
        var message = MessageBuilder.BuildRoomInfoMessage(user.SelectedRoom!);
        return Notifications.SendMessage(chat.Id, message);
    }

    private Task CommandLeaveRoom(Chat chat, UserEntity user, string[] args)
    {
        return !user.IsAdmin()
            ? Notifications.SendErrorMessage(chat.Id, Localization.AdminCantLeaveRoom)
            : _innerStates[LEAVE_TITLE].PrepareState(user, args);
    }

    private async Task CommandLeaveRoomConfirmation(Chat chat, UserEntity user, string[] args)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;

        Database.RemoveUserFromRoom(user, room);

        var message = MessageBuilder.BuildLeaveMessage(participant);
        await Notifications.NotifyEveryoneInRoom(room, message);
        await Notifications.SendMessage(chat.Id, Localization.UserLeavedRoom);
    }



    private Task CommandShowMe(Chat chat, UserEntity user, string[] args)
    {
        var target = user.GetAsParticipant()!;
        return ShowUserInfo(user.Id, target,
            Localization.UserWishesList,
            Localization.UserHaveZeroWishes);
    }

    private Task CommandShowTarget(Chat chat, UserEntity user, string[] args)
    {
        var me = user.GetAsParticipant()!;
        if (me.TargetUserId is null)
            return Notifications.SendErrorMessage(chat.Id, Localization.SecretSantaStillOffline);

        var room = user.SelectedRoom!;
        var target = room.Participants.First(x => x.Id == me.TargetUserId.Value);
        return ShowUserInfo(user.Id, target,
            Localization.TargetWishesList,
            Localization.TargetHaveZeroWishes);
    }

    private async Task ShowUserInfo(long chatId, ParticipantEntity target, string header, string emptyMsg)
    {
        var message = MessageBuilder.BuildUserInfoMessage(header, target);
        await Notifications.SendMessage(chatId, message);

        foreach (var wish in target.Wishes.Where(x => x.Images is { Count: > 0 }))
            await Notifications.SendImages(chatId, wish.Images, wish.Message);
    }
}
