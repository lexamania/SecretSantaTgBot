using System.Globalization;

using SecretSantaTgBot.Attributes;

namespace SecretSantaTgBot.Services.Helpers;

[RequiredService(InjectionType.Singleton)]
public sealed class LocalizationService
{
    public IDisposable UseCulture(string language)
        => new CultureScope(language);

    private sealed class CultureScope : IDisposable
    {
        public CultureScope (string language)
        {
            _previousCulture = CultureInfo.CurrentCulture;
            _previousUICulture = CultureInfo.CurrentUICulture;

            var culture = CultureInfo.GetCultureInfo(language);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        private readonly CultureInfo _previousCulture;
        private readonly CultureInfo _previousUICulture;

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _previousCulture;
            CultureInfo.CurrentUICulture = _previousUICulture;
        }
    }
}
