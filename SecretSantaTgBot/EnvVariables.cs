using SecretSantaTgBot.Messages;

namespace SecretSantaTgBot;

public static class EnvVariables
{
    private static readonly MessagesDictionary _msgDict = new();

    public static string BotToken { get; }
    public static string DBPath { get; }
    public static string BotName { get; set; }
    public static string Language  { get; } = "UA";
    public static MessagesBase Messages => _msgDict[Language];

    static EnvVariables()
    {
        // Expect .env file in main directory 
        DotNetEnv.Env.TraversePath().Load();

        BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!;
        DBPath = Path.Combine(AppContext.BaseDirectory, @"Data\Storage.db");
    }
}
