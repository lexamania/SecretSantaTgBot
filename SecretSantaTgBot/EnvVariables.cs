namespace SecretSantaTgBot;

public static class EnvVariables
{
    public static string BotToken { get; }
    public static string DBPath { get; }
    public static string ImagesPath { get; }

    static EnvVariables()
    {
        // Expect .env file in main directory 
        DotNetEnv.Env.TraversePath().Load();

        BotToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!;
        DBPath = Path.Combine(
                    AppContext.BaseDirectory, 
                    Environment.GetEnvironmentVariable("STORAGE_PATH")!);
        ImagesPath = Path.Combine(
                    AppContext.BaseDirectory, 
                    Environment.GetEnvironmentVariable("IMAGE_STORAGE_PATH")!);
    }
}
