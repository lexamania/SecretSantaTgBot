namespace SecretSantaTgBot.Enums;

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
        public static string? GetKey(string command) => command switch
        {
            Start => BotContent.Commands.Start,
            Help => BotContent.Commands.Help,
            Back => BotContent.Commands.BackToMenu,
            Stop => BotContent.Commands.Stop,
            CreateRoom => BotContent.Commands.CreateRoom,
            SelectRoom => BotContent.Commands.SelectRoom,
            DeleteRoom => BotContent.Commands.DeleteRoom,
            ShowRooms => BotContent.Commands.ShowRooms,
            ShowMe => BotContent.Commands.ShowMe,
            ShowMyTarget => BotContent.Commands.ShowTarget,
            ShowRoomInfo => BotContent.Commands.ShowRoomInfo,
            AddWishes => BotContent.Commands.AddWishes,
            ClearWishes => BotContent.Commands.ClearWishes,
            LeaveRoom => BotContent.Commands.LeaveRoom,
            ModifyRoom => BotContent.Commands.ModifyRoom,
            NotifyEveryone => BotContent.Commands.NotifyEveryone,
            StartSanta => BotContent.Commands.StartSanta,
            _ => default
        };
    }
}
