using SecretSanta.TelegramApi.Models;
using SecretSanta.TelegramApi.Services;

namespace SecretSanta.TelegramApi.Middleware;

public sealed class TelegramInitDataMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        TelegramInitDataValidator validator)
    {
        if (!context.Request.Path.StartsWithSegments("/api")
            || context.Request.Path.StartsWithSegments("/api/health"))
        {
            await next(context);
            return;
        }

        if (!validator.TryValidate(
                context.Request.Headers["X-Telegram-Init-Data"],
                out var telegramInitData))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        context.Items[typeof(TelegramInitData)] = telegramInitData;
        await next(context);
    }
}
