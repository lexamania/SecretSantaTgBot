using SecretSantaTgBot.Models.Commands.Base;

namespace SecretSantaTgBot.Extensions;

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
