namespace SecretSantaTgBot.Utils;

public static class NameParser
{
    public static string[] ParseArgs(string text)
        => text.Split(' ').Where(x => x.Length > 0).ToArray();

    public static string[] ParseButton(string text)
        => text.Split().Where(x => x.Length > 0).ToArray();

    public static string JoinArgs(params object[] args)
        => string.Join(' ', args);

    public static string GetRoomJoinLink(string roomId)
        => $"t.me/secret_santa_lexamania_bot?start={roomId}";
}
