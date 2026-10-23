namespace SecretSantaTgBot.Utils;

public static class NameParser
{
    public static string[] ParseButton(string text)
        => text.Split().Where(x => x.Length > 0).ToArray();

    public static string JoinArgs(params string[] args)
        => string.Join(' ', args);
}
