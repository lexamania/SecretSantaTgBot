namespace SecretSantaTgBot.Enums;

public static class StatesList
{
    public const string Global = "global";
    public const string Default = "default";
    public const string InRoom = "in_room";

    public const string RoomCreation = "room_creation";
    public const string RoomDeleting = "room_deleting";
    public const string RoomSelection = "room_selection";
    public const string RoomRegistration = "room_registration";

    public const string RoomLeaving = "room_leaving";
    public const string RoomModifying = "room_modifying";
    public const string AddWishes = "add_wishes";
    public const string NotifyEveryone = "notify_everyone";

    public const string Confirmation = "confirmation";
    public const string EnterTitle = "enter_title";
    public const string EnterDescription = "enter_description";
    public const string EnterInRoomName = "enter_name";
    public const string EnterRoomAlias = "enter_room_alias";
}
