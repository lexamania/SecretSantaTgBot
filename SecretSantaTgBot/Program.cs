using SecretSantaTgBot;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

using Telegram.Bot;

var botToken = EnvVariables.BotToken;

using var cts = new CancellationTokenSource();
using var db = new SantaDatabase();

var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var notifyService = new NotificationService(bot);
var logger = new LocalLogger();

var errorBroker = new ErrorBrokerService(cts);
var msgBroker = new MessageBrokerService(db, notifyService, logger);
var queryBroker = new QueryBrokerService(bot, db);

var startUsers = db.Users.FindAll().Select(x => x.Id).ToArray();
await notifyService.NotifyEveryone(startUsers, "Бот знову активний!", true);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;
bot.OnUpdate += queryBroker.OnUpdate;

var me = await bot.GetMe();
EnvVariables.BotName = me.Username!;

Console.WriteLine($"@{me.Username} is running... Send STOP to terminate");

while (Console.ReadLine() != "STOP") ;

var endUsers = db.Users.FindAll().Select(x => x.Id).ToArray();
await notifyService.NotifyEveryone(startUsers, "Бот відключено!", true);
cts.Cancel(); // stop the bot
