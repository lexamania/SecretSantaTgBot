const telegram = window.Telegram?.WebApp;

function applyTelegramTheme(themeParams) {
  if (!themeParams) return;

  const root = document.documentElement;
  const theme = telegram?.colorScheme;

  if (theme === "light" || theme === "dark") {
    root.dataset.telegramTheme = theme;
    root.style.colorScheme = theme;
  }

  const colors = {
    "--page": themeParams.bg_color,
    "--surface": themeParams.secondary_bg_color,
    "--text": themeParams.text_color,
    "--muted": themeParams.hint_color,
  };

  for (const [property, value] of Object.entries(colors)) {
    if (typeof value === "string" && /^#[\da-f]{6}$/i.test(value)) {
      root.style.setProperty(property, value);
    }
  }
}

export function initializeTelegram() {
  if (!telegram) return null;

  applyTelegramTheme(telegram.themeParams);
  telegram.ready();
  telegram.expand();
  telegram.onEvent("themeChanged", () => applyTelegramTheme(telegram.themeParams));

  return telegram;
}
