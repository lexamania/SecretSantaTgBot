using Microsoft.Extensions.Localization;

using SecretSanta.Localization.Services;

namespace SecretSanta.Localization.Localizers;

public sealed class WebAppLocalizer(
    IStringLocalizer<WebAppContent> localizer,
    CultureSwitcher cultureSwitcher)
{
    public IReadOnlyDictionary<string, string> GetStrings(string? requestedLanguage)
    {
        var language = LanguageManager.GetSupportedLanguage(requestedLanguage);

        using (cultureSwitcher.UseCulture(language))
        {
            return new Dictionary<string, string>
            {
                ["language"] = language,
                ["brand"] = localizer["webapp_brand"].Value,
                ["brandHome"] = localizer["webapp_brand_home"].Value,
                ["season"] = localizer["webapp_season"].Value,
                ["heroEyebrow"] = localizer["webapp_hero_eyebrow"].Value,
                ["heroTitleFirstLine"] = localizer["webapp_hero_title_first_line"].Value,
                ["heroTitleSecondLine"] = localizer["webapp_hero_title_second_line"].Value,
                ["welcomeCopy"] = localizer["webapp_welcome_copy"].Value,
                ["roomsEyebrow"] = localizer["webapp_rooms_eyebrow"].Value,
                ["roomsTitle"] = localizer["webapp_rooms_title"].Value,
                ["emptyTitle"] = localizer["webapp_empty_title"].Value,
                ["emptyDescription"] = localizer["webapp_empty_description"].Value,
                ["footer"] = localizer["webapp_footer"].Value,
                ["statusInitializing"] = localizer["webapp_status_initializing"].Value,
                ["statusOpenInTelegram"] = localizer["webapp_status_open_in_telegram"].Value,
                ["statusLoadingRooms"] = localizer["webapp_status_loading_rooms"].Value,
                ["statusRoomsReady"] = localizer["webapp_status_rooms_ready"].Value,
                ["statusNoRooms"] = localizer["webapp_status_no_rooms"].Value,
                ["statusInvalidTelegram"] = localizer["webapp_status_invalid_telegram"].Value,
                ["statusServerNotConfigured"] = localizer["webapp_status_server_not_configured"].Value,
                ["statusConnectionError"] = localizer["webapp_status_connection_error"].Value,
                ["statusRoomsError"] = localizer["webapp_status_rooms_error"].Value,
                ["roomsCountOne"] = localizer["webapp_rooms_count_one"].Value,
                ["roomsCountFew"] = localizer["webapp_rooms_count_few"].Value,
                ["roomsCountMany"] = localizer["webapp_rooms_count_many"].Value,
                ["participantsCountOne"] = localizer["webapp_participants_count_one"].Value,
                ["participantsCountFew"] = localizer["webapp_participants_count_few"].Value,
                ["participantsCountMany"] = localizer["webapp_participants_count_many"].Value,
                ["admin"] = localizer["webapp_admin"].Value,
                ["drawComplete"] = localizer["webapp_draw_complete"].Value
            };
        }
    }
}
