using SecretSantaTgBot.Messages;

namespace SecretSantaTgBot;

public static class EnvVariables
{
    private static readonly MessagesDictionary _msgDict = new();

    public static string BotToken { get; }
    public static string DBPath { get; }
    public static string LogsDirPath { get; }
    public static string Language  { get; } = "UA";
    public static MessagesBase Messages => _msgDict[Language];

    public static string BotName { get; set; }

    static EnvVariables()
    {
        // Expect .env file in main directory 
        DotNetEnv.Env.TraversePath().Load();

        BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!;
        DBPath = Path.Join(AppContext.BaseDirectory, "Data", "Storage.db");
        LogsDirPath = Path.Join(AppContext.BaseDirectory, @"Logs");
    }
}
