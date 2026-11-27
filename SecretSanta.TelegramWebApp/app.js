import { getLocalization, getRooms } from "./api.js";
import { initializeTelegram } from "./telegram.js";
import { renderRooms } from "./rooms.js";

const statusMessage = document.querySelector("#app-status");

function applyLocalization(localization) {
  document.documentElement.lang = localization.language;
  document.title = localization.brand;
  document.querySelector('meta[name="description"]').content = localization.welcomeCopy;
  document.querySelector(".brand").setAttribute("aria-label", localization.brandHome);

  for (const element of document.querySelectorAll("[data-i18n]")) {
    element.textContent = localization[element.dataset.i18n];
  }
}

function setStatus(message) {
  statusMessage.textContent = message;
}

async function startApp() {
  const telegram = initializeTelegram();
  let localization;

  try {
    const language = telegram?.initDataUnsafe?.user?.language_code ?? navigator.language;
    localization = await getLocalization(language);
    applyLocalization(localization);
    setStatus(localization.statusInitializing);
  } catch {
    setStatus("Не вдалося завантажити переклади. Перевірте підключення та спробуйте ще раз.");
    return;
  }

  if (!telegram?.initData) {
    setStatus(localization.statusOpenInTelegram);
    return;
  }

  setStatus(localization.statusLoadingRooms);

  try {
    const rooms = await getRooms(telegram.initData);
    renderRooms(rooms, localization);
    setStatus(rooms.length
      ? localization.statusRoomsReady
      : localization.statusNoRooms);
  } catch (error) {
    if (error.status === 401) {
      setStatus(localization.statusInvalidTelegram);
    } else if (error.status === 503) {
      setStatus(localization.statusServerNotConfigured);
    } else if (error instanceof TypeError) {
      setStatus(localization.statusConnectionError);
    } else {
      setStatus(localization.statusRoomsError);
    }
  }
}

void startApp();
