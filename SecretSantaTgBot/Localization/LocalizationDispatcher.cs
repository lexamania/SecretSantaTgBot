using SecretSantaTgBot.Localization.Dictionaries;

namespace SecretSantaTgBot.Localization;

public class LocalizationDispatcher
{
    private readonly Dictionary<string, ILocalization> _messagesDictionary = [];

    public LocalizationDispatcher()
    {
        _messagesDictionary["UA"] = new LocalizationUA();
    }

    public ILocalization Get(string language)
    {
        return _messagesDictionary.TryGetValue(language, out var messagesDictionary)
            ? messagesDictionary
            : _messagesDictionary["UA"];
    }
}
