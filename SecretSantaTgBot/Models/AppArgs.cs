namespace SecretSantaTgBot.Models;

public class AppArgs
{
    public readonly bool IsSilentStart = false;
    public readonly bool IsSilentStop = false;

    public AppArgs(string[] args)
    {
        foreach(var arg in args)
        {
            var values = arg.Split("=");
            if (values.Length != 2)
                continue;

            _ = values[0] switch
            {
                "start" => bool.TryParse(values[1], out IsSilentStart),
                "stop" => bool.TryParse(values[1], out IsSilentStop),
                _ => false
            };
            
        }
    }
}