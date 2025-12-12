using SecretSantaTgBot;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

using Telegram.Bot;

var botToken = EnvVariables.BotToken;
var appArgs = new AppArgs(args);

using var cts = new CancellationTokenSource();
using var db = new SantaDatabase();

var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var notifyService = new NotificationService(bot);
var logger = new LocalLogger();

var errorBroker = new ErrorBrokerService(cts);
var msgBroker = new MessageBrokerService(db, notifyService, logger);
var queryBroker = new QueryBrokerService(bot, db);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;
bot.OnUpdate += queryBroker.OnUpdate;

if (!appArgs.IsSilentStart)
{
    var startUsers = db.Users.FindAll().Select(x => x.Id).ToArray();
    await notifyService.NotifyEveryone(startUsers, "Бот знову активний!", true);
}

var me = await bot.GetMe();
EnvVariables.BotName = me.Username!;

Console.WriteLine($"@{me.Username} is running... Send STOP to terminate");

while (Console.ReadLine() != "STOP") ;

if (!appArgs.IsSilentStop)
{
    var endUsers = db.Users.FindAll().Select(x => x.Id).ToArray();
    await notifyService.NotifyEveryone(endUsers, "Бот відключено!", true);
}

cts.Cancel(); // stop the bot
