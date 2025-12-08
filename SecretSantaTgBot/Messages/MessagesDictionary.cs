namespace SecretSantaTgBot.Messages;

public class MessagesDictionary
{
    private readonly Dictionary<string, MessagesBase> _messages = [];
    
    public MessagesBase this[string lang]
    {
        get => _messages[lang];
        private set => _messages[lang] = value;
    }

    public MessagesDictionary()
    {
        this["UA"] = new MessagesUA();
    }
}
