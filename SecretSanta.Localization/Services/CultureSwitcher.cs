using System.Globalization;

namespace SecretSanta.Localization.Services;

public class CultureSwitcher
{
    public IDisposable UseCulture(string language)
        => new CultureScope(language);

    private sealed class CultureScope : IDisposable
    {
        public CultureScope(string language)
        {
            var culture = CultureInfo.GetCultureInfo(language);

            _previousCulture = CultureInfo.CurrentCulture;
            _previousUICulture = CultureInfo.CurrentUICulture;

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
