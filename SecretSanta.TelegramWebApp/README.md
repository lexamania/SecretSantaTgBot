# Secret Santa Telegram Mini App

The Mini App frontend is built with plain HTML, CSS, and JavaScript. It has no
build step or package dependencies. The page currently lists rooms from the
ASP.NET Core API; room creation and other actions are not implemented yet.

## Frontend files

- `app.js` starts the app and coordinates page state.
- `telegram.js` initializes the Telegram Mini App SDK and applies Telegram
  theme colors.
- `api.js` contains shared API request helpers and error handling.
- `rooms.js` renders the room list.

Add feature-specific modules (for example `wishes.js`) when those screens and
API operations are implemented.

## Run locally with .NET

Set the same bot token used by the Telegram bot so the API can validate Mini
App sessions:

```sh
export TelegramBot__Token="your-bot-token"
```

From the repository root, start the ASP.NET Core host:

```sh
dotnet run --project SecretSanta.TelegramApi
```

Then open `http://localhost:5080`. The API project serves these frontend files
from its `wwwroot` path, including when published. Outside Telegram the page
displays a preview notice. Telegram Mini Apps must be served from a public
HTTPS URL when registered for use in Telegram.

## Telegram integration

The page loads Telegram's official `telegram-web-app.js` SDK and uses it for
`ready()`, `expand()`, theme colors, and the signed `initData` needed by the
rooms endpoint. It does not display unverified user identity. The rooms endpoint
accepts Telegram `initData` in the `X-Telegram-Init-Data` header and validates
it on the server before returning the current user's rooms. The local API
defaults to the same LiteDB file as the bot; set `LiteDb__DatabasePath` to
override it.
