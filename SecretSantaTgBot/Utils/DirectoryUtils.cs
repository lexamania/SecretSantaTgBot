namespace SecretSantaTgBot.Utils;

public static class DirectoryUtils
{
    public static void CreateDirectoryRecursively(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
            return;

        CreateDirectoryRecursively(Path.GetDirectoryName(directoryPath)!);
        Directory.CreateDirectory(directoryPath);
    }
}
