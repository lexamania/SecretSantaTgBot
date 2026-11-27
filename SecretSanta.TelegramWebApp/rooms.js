const roomsList = document.querySelector("#rooms-list");
const roomCount = document.querySelector("#room-count");
const emptyRooms = document.querySelector("#empty-rooms");

export function renderRooms(rooms, localization) {
  roomsList.replaceChildren();
  roomCount.textContent = String(rooms.length);
  roomCount.setAttribute(
    "aria-label",
    `${rooms.length} ${getCountLabel(rooms.length, localization, "roomsCount")}`,
  );

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
      `${room.participantCount} ${getCountLabel(
        room.participantCount,
        localization,
        "participantsCount",
      )}`,
    ));

    if (room.isAdmin) {
      metadata.append(createBadge(localization.admin));
    }

    if (room.drawStarted) {
      metadata.append(createBadge(localization.drawComplete, "drawn"));
    }

    card.append(metadata);
    roomsList.append(card);
  }
}

function getCountLabel(count, localization, keyPrefix) {
  const pluralCategory = new Intl.PluralRules(localization.language).select(count);
  return localization[`${keyPrefix}${pluralCategory[0].toUpperCase()}${pluralCategory.slice(1)}`];
}

function createBadge(label, modifier = "") {
  const badge = document.createElement("span");
  badge.className = modifier ? `room-badge ${modifier}` : "room-badge";
  badge.textContent = label;
  return badge;
}
