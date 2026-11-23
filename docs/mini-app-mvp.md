# Telegram Mini App MVP brief

This is the shared product and integration brief for work on
`SecretSanta.TelegramWebApp` and `SecretSanta.TelegramApi`. It records the
decisions made so far and calls out unresolved details so implementation agents
do not silently invent product behavior.

## Product direction

- Build a Telegram Mini App with vanilla JavaScript, HTML, and CSS; do not add
  React or a frontend framework for the MVP.
- Keep the existing Telegram bot. The Mini App complements it rather than
  replacing its commands.
- `SecretSanta.TelegramWebApp` is the frontend folder.
- `SecretSanta.TelegramApi` is the ASP.NET Core API project. Its current
  `Program.cs` is a console placeholder, not a working web API.
- The frontend and API may be implemented in parallel after agreeing on their
  shared HTTP contract.

## MVP capabilities

Users should be able to:

1. See their rooms and create/manage rooms.
2. Open a room from an admin-generated invite link that can be shared.
   Non-members see room details and enter their in-room name to confirm
   joining; opening an invite does not silently add them.
3. See room name, description, participant list, and who the admin is.
4. Manage their own profile within a room and maintain several separate wish
   items. Each wish item supports text and an optional image/photo.
5. After the draw, see their assigned target's in-room name and wish list.

Room creators/admins should be able to:

1. Manage their room and its membership.
2. Start the Secret Santa draw from the Mini App.

The draw can be started once. Starting it locks membership. Assignments are
private and cannot be changed or rerun. A participant must only see their own
assignment; admins must not gain access to other participants' assignments or
wish lists just because they are admins.

Wish lists are visible to their owner and, after the draw, to the assigned
Santa. They are not visible to other room members or room admins by default.

## Existing implementation to preserve and reuse

- Shared room/user storage and domain code live in `SecretSanta.Shared`.
- Persistence is accessed through `ISantaDatabase`, `IUserRepository`, and
  `IRoomRepository`; the current implementation is LiteDB. Repository APIs use
  async methods so another provider can be substituted without changing the
  bot/API consumers. LiteDB itself is synchronous, so its adapter still runs
  the underlying operations synchronously.
- `PartyRoomEntity` already has a title, description, admin, participants, and
  a `SantaPlayed` flag.
- `ParticipantEntity` already has a Telegram user ID, username, custom in-room
  name, wishes, and target user ID.
- A wish currently contains a message and image IDs. Preserve compatibility
  with existing bot data and behavior; do not replace the storage model without
  checking all bot callers and deciding how legacy wishes map to the UI.
- The bot and API must continue to use compatible room, membership, wish, and
  draw behavior. API operations must not corrupt data the bot expects.

These are implementation clues, not permission to expose storage entities
directly as HTTP responses.

## Security and privacy requirements

- Authenticate Mini App requests by validating Telegram `initData` on the
  server. Do not trust a Telegram user ID, role, room membership, or other
  identity supplied separately by the browser.
- Apply authorization on every room/member/wish/draw operation. Only room
  admins may manage room settings/membership or start the draw; users may
  change only their own profile and wishes.
- Enforce wish-list and target-assignment visibility on the server, not only
  by hiding UI elements.
- Never return other participants' target assignments. Do not expose a whole
  room's wish lists through general room or participant-list responses.
- Use HTTPS for the deployed Mini App and API. Do not put bot tokens or other
  server secrets in frontend files.

## Parallel implementation workflow

1. Inspect existing bot commands, domain methods, and persistence behavior
   before defining API operations. Reuse shared domain behavior where
   appropriate.
2. Agree on a versioned request/response contract and error format before the
   frontend and API agents implement against each other. Include Telegram
   authentication, room access from bot invite links, wish item shape, and
   draw state in that contract.
3. The API agent implements server-side Telegram validation, authorization,
   endpoints, and focused tests.
4. The frontend agent implements the Mini App UI and Telegram WebApp
   integration, using only the agreed API contract. Keep API base URL
   configurable for local development and deployment.
5. Integrate and verify the flows end-to-end, including privacy, membership
   locking, and compatibility with the bot.

## Decisions still needed before the implementation contract is final

- Invite token/link lifecycle: expiration, revocation, and whether an invite
  has a usage limit. The link must convey room context safely and must not by
  itself bypass the confirmation and authorization checks above.
- Room edit/delete behavior and the exact membership actions available to an
  admin; also whether any self-service leave action is allowed and under what
  conditions.
- The fields in a user's room profile beyond their in-room display name, if
  any.
- Any limit on the number of wish items.
- Whether existing bot image wishes can be displayed/reused directly in the
  Mini App or need an API upload/display flow.
- Minimum participant count and any other validation before an admin can
  start the draw.
- Deployment/hosting topology and local development origins. The API and
  frontend can be separate projects, but the final setup must configure
  HTTPS, CORS (if cross-origin), and the Mini App URL consistently.
- Whether existing Telegram bot UI provides a way to open the Mini App, and
  which bot commands remain the entry points for the current flows.

Do not treat unresolved items as settled product decisions. Ask the product
owner or record an explicit, reversible proposal before depending on them.
