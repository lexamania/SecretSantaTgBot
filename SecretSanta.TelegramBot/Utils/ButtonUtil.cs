using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Storage.Entities;

namespace SecretSanta.TelegramBot.Utils;

public static class ButtonUtil
{
    public static string GenerateRoomButton(PartyRoomEntity room)
        => $"{room.Title} {room.Id}";

    public static string[] ParseButtonMessage(string text)
        => [.. text.Split().Where(x => x.Length > 0)];

    public static string ParseRoomIdFromAlias(string roomAlias)
        => ParseButtonMessage(roomAlias).Last();

    public static string[] GetCommandsAsButtons(IMessageState state)
        => [..state.ReadOnlyCommands.Keys];

    public static string[] GetAdminRoomButtons(MessageContext context)
        => context.User.GetAdminRooms()
            .Select(GenerateRoomButton)
            .ToArray()!;

    public static string[] GetRoomButtons(MessageContext context)
        => context.User.AvailableRooms
            .Select(GenerateRoomButton)
            .ToArray()!;
}
