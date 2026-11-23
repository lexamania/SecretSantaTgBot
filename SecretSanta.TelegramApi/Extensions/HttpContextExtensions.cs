using SecretSanta.TelegramApi.Models;

namespace SecretSanta.TelegramApi.Extensions;

public static class HttpContextExtensions
{
    public static TelegramInitData GetTelegramInitData(this HttpContext context)
        => context.Items.TryGetValue(typeof(TelegramInitData), out var value)
            && value is TelegramInitData initData
                ? initData
                : throw new InvalidOperationException(
                    "Validated Telegram init data is not available for this request.");
}
