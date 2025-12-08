using SecretSantaTgBot;
using SecretSantaTgBot.Messages;
using SecretSantaTgBot.Storage;

using Telegram.Bot;

var botToken = EnvVariables.BotToken;

using var cts = new CancellationTokenSource();
using var db = new SantaDatabase();

var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var msgDict = new MessagesDictionary();
var errorBroker = new ErrorBroker(cts);
var msgBroker = new MessageBroker(bot, db, msgDict);
var queryBroker = new QueryBroker(bot, db, msgDict);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;
bot.OnUpdate += queryBroker.OnUpdate;

var me = await bot.GetMe();

Console.WriteLine($"@{me.Username} is running... Send Alt+Num1 to terminate");

while (Console.Read() != '☺') ;

cts.Cancel(); // stop the bot
