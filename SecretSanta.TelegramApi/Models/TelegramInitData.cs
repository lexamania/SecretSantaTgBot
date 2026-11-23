using System.Text.Json.Serialization;

namespace SecretSanta.TelegramApi.Models;

public sealed record TelegramInitData(
    TelegramUser User,
    DateTimeOffset AuthDate,
    string? QueryId,
    string? StartParam,
    string? ChatType,
    string? ChatInstance);

public sealed record TelegramUser(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("last_name")] string? LastName,
    string? Username,
    [property: JsonPropertyName("language_code")] string? LanguageCode,
    [property: JsonPropertyName("is_premium")] bool? IsPremium,
    [property: JsonPropertyName("photo_url")] string? PhotoUrl);
