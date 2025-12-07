using SecretSantaTgBot;
using SecretSantaTgBot.Storage;

using Telegram.Bot;

var botToken = EnvVariables.BotToken;

using var cts = new CancellationTokenSource();
using var db = new SantaDatabase();

var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var errorBroker = new ErrorBroker(cts);
var msgBroker = new MessageBroker(bot, db);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;

var me = await bot.GetMe();

Console.WriteLine($"@{me.Username} is running... Send Alt+Num1 to terminate");

while (Console.Read() != '☺') ;

cts.Cancel(); // stop the bot
