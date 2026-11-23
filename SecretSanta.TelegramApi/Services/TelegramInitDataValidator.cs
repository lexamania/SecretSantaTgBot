using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using SecretSanta.TelegramApi.Attributes;
using SecretSanta.Shared.Configurations;
using SecretSanta.TelegramApi.Models;

namespace SecretSanta.TelegramApi.Services;

[RequiredService(InjectionType.Singleton)]
public sealed class TelegramInitDataValidator(IOptions<TelegramBotOptions> options)
{
    private static readonly TimeSpan MaxAge = TimeSpan.FromHours(24);

    public bool TryValidate(string? initData, out TelegramInitData? validatedData)
    {
        validatedData = null;

        if (string.IsNullOrWhiteSpace(initData))
        {
            return false;
        }

        var parameters = QueryHelpers.ParseQuery(initData);
        if (parameters.Any(parameter => parameter.Value.Count != 1)
            || !TryGetSingleValue(parameters, "hash", out var receivedHash)
            || !TryGetSingleValue(parameters, "auth_date", out var authDateValue)
            || !TryGetSingleValue(parameters, "user", out var userJson)
            || receivedHash.Length != 64
            || !long.TryParse(authDateValue, NumberStyles.None, CultureInfo.InvariantCulture, out var authDate))
        {
            return false;
        }

        byte[] receivedHashBytes;
        try
        {
            receivedHashBytes = Convert.FromHexString(receivedHash);
        }
        catch (FormatException)
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var maxAgeSeconds = (long)MaxAge.TotalSeconds;
        if (authDate > now + 60 || now - authDate > maxAgeSeconds)
        {
            return false;
        }

        var dataCheckString = string.Join(
            '\n',
            parameters
                .Where(parameter => parameter.Key is not ("hash" or "signature"))
                .OrderBy(parameter => parameter.Key, StringComparer.Ordinal)
                .Select(parameter => $"{parameter.Key}={parameter.Value}"));

        var secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            Encoding.UTF8.GetBytes(options.Value.Token));
        var expectedHash = HMACSHA256.HashData(
            secretKey,
            Encoding.UTF8.GetBytes(dataCheckString));

        if (!CryptographicOperations.FixedTimeEquals(receivedHashBytes, expectedHash))
        {
            return false;
        }

        try
        {
            var user = JsonSerializer.Deserialize<TelegramUser>(userJson);
            if (user is null || user.Id <= 0 || string.IsNullOrWhiteSpace(user.FirstName))
            {
                return false;
            }

            DateTimeOffset authDateTime;
            try
            {
                authDateTime = DateTimeOffset.FromUnixTimeSeconds(authDate);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            validatedData = new TelegramInitData(
                user,
                authDateTime,
                GetOptionalValue(parameters, "query_id"),
                GetOptionalValue(parameters, "start_param"),
                GetOptionalValue(parameters, "chat_type"),
                GetOptionalValue(parameters, "chat_instance"));
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryGetSingleValue(
        IDictionary<string, StringValues> parameters,
        string key,
        out string value)
    {
        value = string.Empty;
        if (!parameters.TryGetValue(key, out var values) || values.Count != 1)
        {
            return false;
        }

        value = values[0] ?? string.Empty;
        return value.Length > 0;
    }

    private static string? GetOptionalValue(
        IDictionary<string, StringValues> parameters,
        string key)
        => parameters.TryGetValue(key, out var values) && values.Count == 1
            ? values[0]
            : null;
}
