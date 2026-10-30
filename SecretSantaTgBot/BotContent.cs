namespace SecretSantaTgBot;

public sealed class BotContent;

public static class BotContentKeys
{
    public static class Commands
    {
        public const string Help = "cmd_help";
        public const string Stop = "cmd_stop";
        public const string CreateRoom = "cmd_create_room";
        public const string SelectRoom = "cmd_select_room";
        public const string DeleteRoom = "cmd_delete_room";
        public const string ShowRooms = "cmd_show_rooms";
        public const string ModifyRoom = "cmd_modify_room";
        public const string LeaveRoom = "cmd_leave_room";
        public const string ShowRoomInfo = "cmd_show_room_info";
        public const string ShowMe = "cmd_show_me";
        public const string ShowTarget = "cmd_show_target";
        public const string AddWishes = "cmd_add_wishes";
        public const string ClearWishes = "cmd_clear_wishes";
        public const string BackToMenu = "cmd_back_to_menu";
        public const string StartSanta = "cmd_start_santa";
        public const string NotifyEveryone = "cmd_notify_everyone";
    }

    public static class Messages
    {
        public const string Menu = "msg_menu";
        public const string ErrCommand = "msg_err_command";
        public const string EnterTitle = "msg_enter_title";
        public const string EnterDescription = "msg_enter_description";
        public const string EnterName = "msg_enter_name";
        public const string RoomCreated = "msg_room_created";
        public const string RoomLink = "msg_room_link";
        public const string RoomModifyed = "msg_room_modifyed";
        public const string ChooseRoom = "msg_choose_room";
        public const string RoomsList = "msg_rooms_list";
        public const string RoomDeleted = "msg_room_deleted";
        public const string ErrZeroRooms = "msg_err_zero_rooms";
        public const string ErrRoomDoesntExist = "msg_err_room_doesnt_exist";
        public const string UserTarget = "msg_user_target";
        public const string WishesList = "msg_wishes_list";
        public const string ErrSantaIsStillOffline = "msg_err_santa_is_still_offline";
        public const string UserAddWishes = "msg_user_add_wishes";
        public const string UserWishIsAdded = "msg_user_wish_is_added";
        public const string ErrNeedAdminRights = "msg_err_need_admin_rights";
        public const string ErrAdminCantLeaveRoom = "msg_err_admin_cant_leave_room";
        public const string UserStartRegistration = "msg_user_start_registration";
        public const string UserRegistered = "msg_user_registered";
        public const string ParticipantsList = "msg_participants_list";
        public const string StartSanta = "msg_start_santa";
        public const string NtfSantaFinished = "msg_ntf_santa_finished";
        public const string ErrSantaNotEnoughParticipants = "msg_err_santa_not_enough_participants";
        public const string ErrSantaWasPlayed = "msg_err_santa_was_played";
        public const string UserLeftRoom = "msg_user_left_room";
        public const string NtfUserLeftRoom = "msg_ntf_user_left_room";
        public const string EnterMessage = "msg_enter_message";
        public const string MessageIsSent = "msg_message_is_sent";
        public const string MessageAskConfirmation = "msg_message_ask_confirmation";
    }

    public static class Buttons
    {
        public const string Yes = "btn_yes";
        public const string No = "btn_no";
    }
}
