using Microsoft.Extensions.Configuration;

namespace SecretSanta.TelegramBot.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration config)
    {
        public T GetRequiredValue<T>(string key)
            => config.GetValue<T>(key)
                ?? throw new ArgumentNullException(key);
    }
}
