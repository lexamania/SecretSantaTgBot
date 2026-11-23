const statusMessage = document.querySelector("#app-status");
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

if (telegram) {
  applyTelegramTheme(telegram.themeParams);
  telegram.ready();
  telegram.expand();
  statusMessage.textContent = "The Mini App is open in Telegram. Room features are coming soon.";
} else {
  statusMessage.textContent = "Preview mode: open this page from Telegram to use it as a Mini App.";
}

telegram?.onEvent("themeChanged", () => applyTelegramTheme(telegram.themeParams));
