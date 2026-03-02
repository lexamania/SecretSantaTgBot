using SecretSantaTgBot;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services;
using SecretSantaTgBot.Storage;

using Telegram.Bot;

var botToken = EnvVariables.BotToken;

using var cts = new CancellationTokenSource();
using var db = new SantaDatabase();

var bot = new TelegramBotClient(botToken, cancellationToken: cts.Token);
var notifyService = new NotificationService(bot);
var logger = new LocalLogger();

var container = new ServiceContainer(
    Database: db,
    Notification: notifyService,
    Logger: logger
);

var errorBroker = new ErrorBrokerService(container);
var msgBroker = new MessageBrokerService(container);
var queryBroker = new QueryBrokerService(container);

bot.OnError += errorBroker.OnError;
bot.OnMessage += msgBroker.OnMessage;
bot.OnUpdate += queryBroker.OnUpdate;

var me = await bot.GetMe();
EnvVariables.BotName = me.Username!;

Console.WriteLine($"@{me.Username} is running... Send STOP to terminate");

while (Console.ReadLine() != "STOP") ;

cts.Cancel(); // stop the bot
