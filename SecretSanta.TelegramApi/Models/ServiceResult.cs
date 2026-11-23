namespace SecretSanta.TelegramApi.Models;

public enum ServiceResultStatus
{
    Success,
    NotFound,
    Conflict,
    Invalid
}

public sealed record ServiceResult<T>(
    ServiceResultStatus Status,
    T? Value = default,
    string? Error = null)
{
    public static ServiceResult<T> Success(T value) => new(ServiceResultStatus.Success, value);

    public static ServiceResult<T> NotFound(string? error = null) =>
        new(ServiceResultStatus.NotFound, Error: error);

    public static ServiceResult<T> Conflict(string? error = null) =>
        new(ServiceResultStatus.Conflict, Error: error);

    public static ServiceResult<T> Invalid(string error) =>
        new(ServiceResultStatus.Invalid, Error: error);
}
