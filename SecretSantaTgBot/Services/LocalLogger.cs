using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services;

public class LocalLogger
{
    private readonly string _dir;
    private const string BASE_FILE_NAME = "Logging";
    private const string FILE_EXT = ".txt";
    private readonly object _loker = new();

    private string CurrentFile
    {
        get
        {
            var date = DateTime.Now.ToString("yyyy_MM_dd");
            return $"{BASE_FILE_NAME}_{date}{FILE_EXT}";
        }
    }

    private string CurrentFilePath
    {
        get
        {
            var filePath = Path.Join(_dir, CurrentFile);
            lock(_loker)
            {
                if(!File.Exists(filePath))
                    File.Create(filePath).Dispose();
            }
            return filePath;
        }
    }

    public LocalLogger()
    {
        _dir = EnvVariables.LogsDirPath;
        DirectoryUtils.CreateDirectoryRecursively(_dir);
    }

    public void LogMessage(Message msg)
    {
        var text = msg.Text is not null && msg.Text.StartsWith('/')
            ? msg.Text
            : msg.Type.ToString();
        var message = $"Received a message: \"{text}\" in '{msg.Chat}'";
        WriteToFile(message);
    }

    public void LogError(Exception ex)
    {
        var message = $"__ERROR__ {ex.Message}";
        WriteToFile(message);
    }

    public void LogUnrecognizedMessage(Message msg)
    {
        var message = $"Received a message of type {msg.Type}";
        WriteToFile(message);
    }

    private string WrapMessage(string msg)
    {
        var time = DateTime.Now.ToString("hh:mm:ss");
        return $"{time} {msg}";
    }

    private void WriteToFile(string message)
    {
        message = WrapMessage(message);
        var filePath = CurrentFilePath;

        lock(_loker)
        {
            using var stream = new StreamWriter(filePath, true);
            stream.WriteLine(message);
        }

        Console.WriteLine(message);
    }
}
