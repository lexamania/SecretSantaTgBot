using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.Shared.Models;
using SecretSanta.Shared.Storage.Entities;

namespace SecretSanta.TelegramBot.Utils;

public static class ButtonUtil
{
    public static string GenerateRoomButton(PartyRoomEntity room)
        => $"{room.Title} {room.Id}";

    public static string[] ParseButtonMessage(string text)
        => [.. text.Split().Where(x => x.Length > 0)];

    public static RoomId? ParseRoomIdFromAlias(string roomAlias)
    {
        var roomId = ParseButtonMessage(roomAlias).LastOrDefault();
        return RoomId.TryParse(roomId, out var parsedRoomId)
            ? parsedRoomId
            : null;
    }

    public static string[] GetCommandsAsButtons(IMessageState state)
        => [..state.ReadOnlyCommands.Keys];

    public static string[] GetRoomButtons(IEnumerable<PartyRoomEntity> rooms)
        => rooms
            .Select(GenerateRoomButton)
            .ToArray()!;
}
