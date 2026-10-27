using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using SecretSantaTgBot.Attributes;

namespace SecretSantaTgBot.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRequiredServices()
        {
            var types = typeof(Program).Assembly.GetTypes();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttributes<RequiredServiceAttribute>().FirstOrDefault();
                if (attribute is null)
                    continue;

                _ = attribute.InjectionType switch
                {
                    InjectionType.Transient => services.AddTransient(type),
                    InjectionType.Scoped => services.AddScoped(type),
                    InjectionType.Singleton or _ => services.AddSingleton(type),
                };
            }

            return services;
        }
    }
}
