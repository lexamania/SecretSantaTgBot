export class ApiError extends Error {
  constructor(status) {
    super(`API request failed with status ${status}.`);
    this.name = "ApiError";
    this.status = status;
  }
}

export async function getRooms(initData) {
  const response = await fetch("/api/rooms", {
    headers: {
      "X-Telegram-Init-Data": initData,
    },
  });

  if (!response.ok) {
    throw new ApiError(response.status);
  }

  const rooms = await response.json();
  if (!Array.isArray(rooms)) {
    throw new Error("The server returned an unexpected rooms response.");
  }

  return rooms;
}

export async function getLocalization(language) {
  const query = new URLSearchParams({ language });
  const response = await fetch(`/api/localization?${query}`);

  if (!response.ok) {
    throw new ApiError(response.status);
  }

  const localization = await response.json();
  if (!localization || typeof localization !== "object"
      || typeof localization.language !== "string"
      || Object.values(localization).some(value => typeof value !== "string")) {
    throw new Error("The server returned an unexpected localization response.");
  }

  return localization;
}
