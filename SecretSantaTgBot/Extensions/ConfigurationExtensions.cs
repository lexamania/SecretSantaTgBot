using Microsoft.Extensions.Configuration;

namespace SecretSantaTgBot.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration config)
    {
        public T GetRequiredValue<T>(string key)
            => config.GetValue<T>(key)
                ?? throw new ArgumentNullException(key);
    }
}
