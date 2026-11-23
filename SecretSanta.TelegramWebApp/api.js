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
