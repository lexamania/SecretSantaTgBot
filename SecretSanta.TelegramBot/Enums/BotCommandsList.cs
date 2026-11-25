namespace SecretSanta.TelegramBot.Enums;

public static class BotCommandsList
{
    public const string Start = "/start";
    public const string Help = "/help";
    public const string Back = "/back";
    public const string Stop = "/stop";

    public const string CreateRoom = "/create_room";
    public const string SelectRoom = "/select_room";
    public const string DeleteRoom = "/delete_room";
    public const string ShowRooms = "/show_rooms";

    public const string ShowMe = "/me";
    public const string ShowMyTarget = "/my_target";
    public const string ShowRoomInfo = "/show_room";
    public const string AddWishes = "/add_wishes";
    public const string ClearWishes = "/clear_wishes";
    public const string LeaveRoom = "/leave_room";
    public const string ModifyRoom = "/modify_room";
    public const string NotifyEveryone = "/notify_everyone";
    public const string StartSanta = "/start_santa";

    public static class Descriptions
    {
        public static string? Get(string command, BotLocalizer localizer)=> command switch
        {
            Start => localizer.Commands.Start,
            Help => localizer.Commands.Help,
            Back => localizer.Commands.BackToMenu,
            Stop => localizer.Commands.Stop,
            CreateRoom => localizer.Commands.CreateRoom,
            SelectRoom => localizer.Commands.SelectRoom,
            DeleteRoom => localizer.Commands.DeleteRoom,
            ShowRooms => localizer.Commands.ShowRooms,
            ShowMe => localizer.Commands.ShowMe,
            ShowMyTarget => localizer.Commands.ShowTarget,
            ShowRoomInfo => localizer.Commands.ShowRoomInfo,
            AddWishes => localizer.Commands.AddWishes,
            ClearWishes => localizer.Commands.ClearWishes,
            LeaveRoom => localizer.Commands.LeaveRoom,
            ModifyRoom => localizer.Commands.ModifyRoom,
            NotifyEveryone => localizer.Commands.NotifyEveryone,
            StartSanta => localizer.Commands.StartSanta,
            _ => default
        };
    }
}
