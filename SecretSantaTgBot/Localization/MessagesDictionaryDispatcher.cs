using SecretSantaTgBot.Localization.Dictionaries;

namespace SecretSantaTgBot.Localization;

public class MessagesDictionaryDispatcher
{
    private readonly Dictionary<string, IMessagesDictionary> _messagesDictionary = [];

    public MessagesDictionaryDispatcher()
    {
        _messagesDictionary["UA"] = new MessagesDictionaryUA();
    }

    public IMessagesDictionary Get(string language)
    {
        return _messagesDictionary.TryGetValue(language, out var messagesDictionary)
            ? messagesDictionary
            : _messagesDictionary["UA"];
    }
}
