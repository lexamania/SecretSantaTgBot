using Microsoft.Extensions.Localization;

namespace SecretSanta.Localization.Localizers;

public sealed class BotLocalizer(IStringLocalizer<BotContent> localizer)
{
    public CommandLocalizer Commands { get; } = new(localizer);
    public MessageLocalizer Messages { get; } = new(localizer);
    public ButtonLocalizer Buttons { get; } = new(localizer);

    public sealed class CommandLocalizer(IStringLocalizer<BotContent> localizer)
    {
        public string Start => localizer["cmd_start"].Value;
        public string Help => localizer["cmd_help"].Value;
        public string Stop => localizer["cmd_stop"].Value;
        public string CreateRoom => localizer["cmd_create_room"].Value;
        public string SelectRoom => localizer["cmd_select_room"].Value;
        public string DeleteRoom => localizer["cmd_delete_room"].Value;
        public string ShowRooms => localizer["cmd_show_rooms"].Value;
        public string ModifyRoom => localizer["cmd_modify_room"].Value;
        public string LeaveRoom => localizer["cmd_leave_room"].Value;
        public string ShowRoomInfo => localizer["cmd_show_room_info"].Value;
        public string ShowMe => localizer["cmd_show_me"].Value;
        public string ShowTarget => localizer["cmd_show_target"].Value;
        public string AddWishes => localizer["cmd_add_wishes"].Value;
        public string ClearWishes => localizer["cmd_clear_wishes"].Value;
        public string BackToMenu => localizer["cmd_back_to_menu"].Value;
        public string StartSanta => localizer["cmd_start_santa"].Value;
        public string NotifyEveryone => localizer["cmd_notify_everyone"].Value;
    }

    public sealed class MessageLocalizer(IStringLocalizer<BotContent> localizer)
    {
        public string ErrServer => localizer["msg_err_server"].Value;
        public string ErrCommand => localizer["msg_err_command"].Value;
        public string ErrText => localizer["msg_err_text"].Value;
        public string Menu => localizer["msg_menu"].Value;
        public string EnterTitle => localizer["msg_enter_title"].Value;
        public string EnterDescription => localizer["msg_enter_description"].Value;
        public string EnterName => localizer["msg_enter_name"].Value;
        public string RoomCreated => localizer["msg_room_created"].Value;
        public string RoomLink => localizer["msg_room_link"].Value;
        public string RoomAdmin => localizer["msg_room_admin"].Value;
        public string RoomModified => localizer["msg_room_modifyed"].Value;
        public string ChooseRoom => localizer["msg_choose_room"].Value;
        public string RoomsList => localizer["msg_rooms_list"].Value;
        public string RoomDeleted => localizer["msg_room_deleted"].Value;
        public string ErrZeroRooms => localizer["msg_err_zero_rooms"].Value;
        public string ErrRoomDoesntExist => localizer["msg_err_room_doesnt_exist"].Value;
        public string UserTarget => localizer["msg_user_target"].Value;
        public string WishesList => localizer["msg_wishes_list"].Value;
        public string ErrSantaIsStillOffline => localizer["msg_err_santa_is_still_offline"].Value;
        public string UserAddWishes => localizer["msg_user_add_wishes"].Value;
        public string UserWishIsAdded => localizer["msg_user_wish_is_added"].Value;
        public string UserWishesIsCleared => localizer["msg_user_wishes_is_cleared"].Value;
        public string ErrNeedAdminRights => localizer["msg_err_need_admin_rights"].Value;
        public string ErrAdminCantLeaveRoom => localizer["msg_err_admin_cant_leave_room"].Value;
        public string UserStartRegistration => localizer["msg_user_start_registration"].Value;
        public string UserRegistered => localizer["msg_user_registered"].Value;
        public string NewParticipant => localizer["msg_new_participant"].Value;
        public string ErrRoomJoining => localizer["msg_err_room_joining"].Value;
        public string ParticipantsList => localizer["msg_participants_list"].Value;
        public string StartSanta => localizer["msg_start_santa"].Value;
        public string NtfSantaFinished => localizer["msg_ntf_santa_finished"].Value;
        public string ErrSantaNotEnoughParticipants => localizer["msg_err_santa_not_enough_participants"].Value;
        public string ErrSantaWasPlayed => localizer["msg_err_santa_was_played"].Value;
        public string UserLeftRoom => localizer["msg_user_left_room"].Value;
        public string NtfUserLeftRoom => localizer["msg_ntf_user_left_room"].Value;
        public string ErrRoomLeaving => localizer["msg_err_room_leaving"].Value;
        public string EnterMessage => localizer["msg_enter_message"].Value;
        public string MessageIsSent => localizer["msg_message_is_sent"].Value;
        public string MessageAskConfirmation => localizer["msg_message_ask_confirmation"].Value;
    }

    public sealed class ButtonLocalizer(IStringLocalizer<BotContent> localizer)
    {
        public string Yes => localizer["btn_yes"].Value;
        public string No => localizer["btn_no"].Value;
    }
}
