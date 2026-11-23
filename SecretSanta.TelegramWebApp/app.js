import { getRooms } from "./api.js";
import { initializeTelegram } from "./telegram.js";
import { renderRooms } from "./rooms.js";

const statusMessage = document.querySelector("#app-status");

function setStatus(message) {
  statusMessage.textContent = message;
}

async function startApp() {
  const telegram = initializeTelegram();

  if (!telegram?.initData) {
    setStatus("Open this page in Telegram to securely load your rooms.");
    return;
  }

  setStatus("Loading your rooms…");

  try {
    const rooms = await getRooms(telegram.initData);
    renderRooms(rooms);
    setStatus(rooms.length
      ? "Your Secret Santa rooms are ready."
      : "No rooms yet. Create one or open an invite from your bot.");
  } catch (error) {
    if (error.status === 401) {
      setStatus("Telegram could not verify this session. Close and reopen the Mini App.");
    } else if (error.status === 503) {
      setStatus("The server is not configured to authenticate Telegram sessions yet.");
    } else if (error instanceof TypeError) {
      setStatus("Could not connect to the server. Check your connection and try again.");
    } else {
      setStatus("Your rooms could not be loaded. Please try again later.");
    }
  }
}

void startApp();
