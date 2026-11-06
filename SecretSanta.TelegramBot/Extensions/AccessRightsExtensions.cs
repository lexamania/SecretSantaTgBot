using SecretSanta.TelegramBot.Models.Commands.Base;

namespace SecretSanta.TelegramBot.Extensions;

public static class AccessRightsExtensions
{
    extension(AccessRights access)
    {
        public bool HasAccess(bool isAdmin)
            => access == AccessRights.Default
                || (access == AccessRights.Admin && isAdmin)
                || (access == AccessRights.NotForAdmin && !isAdmin);
    }
}
