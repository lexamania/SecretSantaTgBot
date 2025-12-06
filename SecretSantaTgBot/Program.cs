using SecretSantaTgBot;

using Telegram.Bot;

// Expect .env file in main directory 
DotNetEnv.Env.TraversePath().Load();

var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN")!;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var errorBroker = new ErrorBroker(cts);
var msgBroker = new MessageBroker(bot);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;

var me = await bot.GetMe();

Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");

while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;

cts.Cancel(); // stop the bot
