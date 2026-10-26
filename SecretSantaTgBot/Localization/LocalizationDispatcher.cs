using SecretSantaTgBot.Localization.Dictionaries;

namespace SecretSantaTgBot.Localization;

public class LocalizationDispatcher
{
    public const string DEFAULT_LANG = "UA";

    private readonly Dictionary<string, ILocalization> _messagesDictionary = [];

    public LocalizationDispatcher()
    {
        _messagesDictionary["UA"] = new LocalizationUA();
    }

    public ILocalization Get(string? language = DEFAULT_LANG)
    {
        return _messagesDictionary.TryGetValue(language, out var localication)
            ? localication
            : _messagesDictionary["UA"];
    }
}
