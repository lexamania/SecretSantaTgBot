# Secret Santa Telegram Mini App

The Mini App frontend is built with plain HTML, CSS, and JavaScript. It has no
build step or package dependencies. The current page is a responsive shell;
room data and actions will be connected after the API contract is defined.

## Run locally with .NET

From the repository root, start the ASP.NET Core host:

```sh
dotnet run --project SecretSanta.TelegramApi
```

Then open `http://localhost:5080`. The API project serves these frontend files
from its `wwwroot` path, including when published. Outside Telegram the page
displays a preview notice. Telegram Mini Apps must be served from a public
HTTPS URL when registered for use in Telegram.

## Telegram integration

The page loads Telegram's official `telegram-web-app.js` SDK and currently
uses it only for `ready()`, `expand()`, and theme colors. It does not yet send
`initData` to the API or display unverified user identity. Add authentication
and data requests only after agreeing on the API contract; the server must
validate Telegram `initData` before trusting the user.
