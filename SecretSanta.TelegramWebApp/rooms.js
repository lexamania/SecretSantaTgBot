const roomsList = document.querySelector("#rooms-list");
const roomCount = document.querySelector("#room-count");
const emptyRooms = document.querySelector("#empty-rooms");

export function renderRooms(rooms) {
  roomsList.replaceChildren();
  roomCount.textContent = String(rooms.length);
  roomCount.setAttribute("aria-label", `${rooms.length} ${rooms.length === 1 ? "room" : "rooms"}`);

  if (rooms.length === 0) {
    roomsList.append(emptyRooms);
    return;
  }

  for (const room of rooms) {
    const card = document.createElement("article");
    card.className = "room-card";

    const header = document.createElement("div");
    header.className = "room-card-header";

    const title = document.createElement("h3");
    title.textContent = room.title;
    header.append(title);
    card.append(header);

    if (room.description) {
      const description = document.createElement("p");
      description.className = "room-description";
      description.textContent = room.description;
      card.append(description);
    }

    const metadata = document.createElement("div");
    metadata.className = "room-meta";
    metadata.append(createBadge(
      `${room.participantCount} ${room.participantCount === 1 ? "participant" : "participants"}`,
    ));

    if (room.isAdmin) {
      metadata.append(createBadge("Admin"));
    }

    if (room.drawStarted) {
      metadata.append(createBadge("Draw complete", "drawn"));
    }

    card.append(metadata);
    roomsList.append(card);
  }
}

function createBadge(label, modifier = "") {
  const badge = document.createElement("span");
  badge.className = modifier ? `room-badge ${modifier}` : "room-badge";
  badge.textContent = label;
  return badge;
}
