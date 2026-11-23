using System.Reflection;

using SecretSanta.TelegramApi.Attributes;

namespace SecretSanta.TelegramApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services)
    {
        var types = typeof(Program).Assembly.GetTypes();

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RequiredServiceAttribute>();
            if (attribute is null)
            {
                continue;
            }

            _ = attribute.InjectionType switch
            {
                InjectionType.Transient => services.AddTransient(type),
                InjectionType.Scoped => services.AddScoped(type),
                InjectionType.Singleton => services.AddSingleton(type),
                _ => throw new ArgumentOutOfRangeException(nameof(attribute.InjectionType))
            };
        }

        return services;
    }
}
