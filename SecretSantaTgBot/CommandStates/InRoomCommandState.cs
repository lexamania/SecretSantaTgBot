// using System.Text;

// using SecretSantaTgBot.CommandStates;
// using SecretSantaTgBot.Messages;
// using SecretSantaTgBot.Models;
// using SecretSantaTgBot.Storage;
// using SecretSantaTgBot.Storage.Models;

// using Telegram.Bot;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
// using Telegram.Bot.Types.ReplyMarkups;

// namespace SecretSantaTgBot;

// public class InRoomCommandState : CommandStateBase
// {
//     public const string TITLE = "in_room";
    
//     private readonly Dictionary<string, CommandInfo> _commands;
//     private readonly TelegramBotClient _bot;
//     private readonly SantaDatabase _db;
//     private readonly MessagesDictionary _msgDict;
//     private readonly string _lang = "UA";

//     public CommandInfo? GetCommand(string cmd, long chatId)
//     {
//         var command = _commands.GetValueOrDefault(cmd);
//         if (command is not { IsForAdmin: true })
//             return command;

//         var user = _db.Users.FindById(chatId);
//         if (user.IsAdmin)
//             return command;

//         return default;
//     }

//     public InRoomCommandState(TelegramBotClient bot, SantaDatabase db, MessagesDictionary msgDict)
//     {
//         _bot = bot;
//         _db = db;
//         _msgDict = msgDict;

//         _commands = new List<CommandInfo> {
//             new("/start", "Start", CommandStart)
//             {
//                 ShowHelp = false
//             },
//             new("/stop", "Stop Bot", CommandStop)
//             {
//                 ShowHelp = false
//             },
//             new("/help", _msgDict[_lang].CommandHelp, CommandHelp),
//             new("/participate", _msgDict[_lang].CommandParticipate, CommandParticipate)
//             {
//                 OnMessageQuery = OnParticipateCommand
//             },
//             new("/stop_participate", _msgDict[_lang].CommandStopParticipate, CommandStopParticipate),
//             new("/show_participants", _msgDict[_lang].CommandShowParticipants, CommandShowParticipationList),
//             new("/show_my_target", _msgDict[_lang].CommandShowTarget, CommandShowTarget),
//             new("/start_wishes", _msgDict[_lang].CommandStartWishes, CommandStartWishes)
//             {
//                 OnMessageQuery = OnNewWishCommand
//             },
//             new("/stop_wishes", _msgDict[_lang].CommandStopWishes, CommandStopWishes),
//             new("/clear_wishes", _msgDict[_lang].CommandClearWishes, CommandClearWishes),
//             new("/show_my_wishes", _msgDict[_lang].CommandShowMyWishes, CommandShowMyWishes),
//             new("/start_santa", _msgDict[_lang].StartSanta, CommandStartSecretSanta)
//             {
//                 IsForAdmin = true,
//                 ShowHelp = true
//             },
//         }.ToDictionary(x => x.Command);
//     }

//     public override Task OnMessage(Message msg, UserTg user)
//     {
//         var text = msg.Text!;

//         if (!text.StartsWith('/'))
//         {
//             var states = user.CurrentState is not null
//                 ? NameParser.ParseArgs(user.CurrentState!)
//                 : [];
//             if (states.Length > 1)
//             {
//                 if (_innerStates.TryGetValue(states[1], out var innserState))
//                     return innserState.OnMessage(msg, user);
//             }

//             return OnCommandError(msg.Chat);
//         }

//         var words = text.Split(' ').Where(x => x.Length > 0).ToArray();
//         var command = words[0];
//         var args = words.Length > 1
//             ? words[1..]
//             : [];

//         var cmd = _commands.GetValueOrDefault(command);
//         if (cmd is null)
//             return OnCommandError(msg.Chat);

//         user.CurrentState = default;
//         _db.Users.Update(user);

//         return cmd.Callback.Invoke(msg.Chat, user, args);
//     }

//     public Task OnCommandError(Chat chat, UserTg user, string[] args)
//         => _bot.SendMessage(chat, _msgDict[_lang].CommandError);

//     private Task CommandStart(Chat chat, UserTg user, string[] args)
//         => CommandHelp(chat);

//     private Task CommandHelp(Chat chat, UserTg user, string[] args)
//     {
//         var strBldr = new StringBuilder();
//         strBldr.AppendLine($"<b>{_msgDict[_lang].CommandBotMenu}</b>");

//         var user = _db.Users.FindById(chat.Id);
//         var filteredCommand = _commands
//             .Where(x => x.Value.ShowHelp)
//             .Where(x => !x.Value.IsForAdmin || user.IsAdmin);

//         foreach (var command in filteredCommand)
//             strBldr.AppendLine($"{command.Value.Command,-20} - {command.Value.Description}");

//         return _bot.SendMessage(chat,
//                 strBldr.ToString(),
//                 parseMode: ParseMode.Html,
//                 replyMarkup: new ReplyKeyboardRemove());
//     }

//     private Task CommandStop(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.FindById(chat.Id);
//         if (user is not { IsActive: true })
//             return Task.CompletedTask;

//         user.IsActive = false;
//         user.IsAdmin = false;
//         user.LastCommand = null;

//         _db.Users.Delete(chat.Id);

//         return Task.CompletedTask;
//     }



//     private Task CommandParticipate(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.FindById(chat.Id);
//         var partCount = _db.Users.Find(x => x.IsActive).Count();

//         if (!user.IsActive)
//         {
//             user.IsActive = true;
//             user.IsAdmin = partCount == 0;
//             user.LastCommand = "/participate";
//             _db.Users.Update(user);
//             return _bot.SendMessage(chat, _msgDict[_lang].UserNewParticipation);
//         }

//         return _bot.SendMessage(chat, _msgDict[_lang].UserTakeParticipation);
//     }

//     private Task CommandStopParticipate(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.Include(x => x.TargetUser).FindById(chat.Id);
//         if (user is not { IsActive: true })
//             return _bot.SendMessage(chat, _msgDict[_lang].UserDontTakeParticipation);

//         if (user.TargetUser is { })
//             return _bot.SendMessage(chat, _msgDict[_lang].UserCantCancelParticipation);

//         user.IsActive = false;
//         _db.Users.Update(user);

//         return _bot.SendMessage(chat, _msgDict[_lang].UserRemovedFromParticipation);
//     }

//     private Task CommandShowParticipationList(Chat chat, UserTg user, string[] args)
//     {
//         var participants = _db.Users
//             .Include(x => x.User)
//             .Find(x => x.IsActive)
//             .ToList();

//         if (participants.Count == 0)
//             return _bot.SendMessage(chat, _msgDict[_lang].EmptyParticipants);

//         var strBldr = new StringBuilder();
//         strBldr.AppendLine($"<b>{_msgDict[_lang].ParticipantsList}</b>");
//         foreach (var prt in participants)
//             strBldr.AppendLine($" ► {prt.User.FullName} - @{prt.User.Username}");

//         return _bot.SendMessage(chat, strBldr.ToString(), parseMode: ParseMode.Html);
//     }



//     private async Task CommandShowTarget(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.Include(x => x.TargetUser).FindById(chat.Id);
//         if (user is not { IsActive: true })
//         {
//             await _bot.SendMessage(chat, _msgDict[_lang].UserDontTakeParticipation);
//             return;
//         }

//         if (user.TargetUser is null)
//         {
//             await _bot.SendMessage(chat, _msgDict[_lang].SecretSantaStillOffline);
//             return;
//         }

//         await _bot.SendMessage(chat, $"{_msgDict[_lang].UserTarget} @{user.TargetUser.Username}");
//         await ShowWishes(chat.Id, user.TargetUser.Id, _msgDict[_lang].TargetWishesList, _msgDict[_lang].TargetHaveZeroWishes);
//     }



//     private Task CommandStartWishes(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.FindById(chat.Id);
//         user.LastCommand = "/start_wishes";
//         _db.Users.Update(user);
//         return _bot.SendMessage(chat,
//             _msgDict[_lang].UserStartWishes,
//             replyMarkup: new[] { "/stop_wishes" });
//     }

//     private Task CommandStopWishes(Chat chat, UserTg user, string[] args)
//     {
//         var user = _db.Users.FindById(chat.Id);
//         user.LastCommand = null;
//         _db.Users.Update(user);
//         return _bot.SendMessage(chat, _msgDict[_lang].UserStopWishes);
//     }

//     private Task CommandShowMyWishes(Chat chat, UserTg user, string[] args)
//         => ShowWishes(chat.Id, chat.Id, _msgDict[_lang].UserWishesList, _msgDict[_lang].UserHaveZeroWishes);

//     private async Task ShowWishes(long targetChat, long userId, string headerMsg, string emptyMsg)
//     {
//         var wishes = _db.UserWishes.Find(x => x.UserId == userId).ToList();
//         if (wishes.Count == 0)
//         {
//             await _bot.SendMessage(targetChat, emptyMsg);
//             return;
//         }

//         var strBldr = new StringBuilder();
//         strBldr.AppendLine($"<b>{headerMsg}</b>");

//         foreach (var wish in wishes.Where(x => x.Images is null))
//             strBldr.AppendLine($" ► {wish.Message}");

//         await _bot.SendMessage(targetChat, strBldr.ToString(), parseMode: ParseMode.Html);

//         foreach (var wish in wishes.Where(x => x.Images is { Length: > 0 }))
//         {
//             var images = wish.Images!
//                 .Select(x => new InputMediaPhoto(x) { Caption = wish.Message })
//                 .ToArray<IAlbumInputMedia>();
//             await _bot.SendMediaGroup(targetChat, images);
//         }
//     }

//     private Task CommandClearWishes(Chat chat, UserTg user, string[] args)
//     {
//         _db.UserWishes.DeleteMany(x => x.UserId == chat.Id);
//         return _bot.SendMessage(chat, _msgDict[_lang].UserWishesCleared);
//     }



//     private async Task CommandStartSecretSanta(Chat chat, UserTg user, string[] args)
//     {
//         var random = new Random();
//         var participants = _db.Users
//             .Include(x => x.User)
//             .Find(x => x.IsActive)
//             .ToArray();

//         var ap = participants.ToList();

//         for (int i = 0; i < participants.Length; ++i)
//         {
//             var fap = ap.Where(x => x != participants[i]).ToArray();

//             if (fap.Length == 0)
//             {
//                 participants[i].TargetUser = participants[i - 1].TargetUser;
//                 participants[i - 1].TargetUser = ap.Last().TargetUser;
//                 ap.Remove(ap.Last());
//                 continue;
//             }

//             var idx = random.Next(0, fap.Length);
//             participants[i].TargetUser = fap[idx].User;

//             ap.Remove(participants[i]);
//         }

//         _db.Users.Update(participants);

//         foreach (var part in participants)
//         {
//             await CommandShowTarget(new Chat() { Id = part.UserId });
//         }
//     }



//     private Task OnParticipateCommand(Message msg)
//     {
//         var text = msg.Text;
//         if (text is not { Length: > 0 } || text.StartsWith("/"))
//             return _bot.SendMessage(msg.Chat, _msgDict[_lang].EnteredNameError);

//         var user = _db.Users.FindById(msg.Chat.Id);
//         user.RealName = text.Trim();
//         user.LastCommand = null;
//         _db.Users.Update(user);
 
//         return _bot.SendMessage(msg.Chat, _msgDict[_lang].UserParticipationEnd);
//     }

//     private Task OnNewWishCommand(Message msg)
//     {
//         var message = msg.Text ?? string.Empty;
//         var images = msg.Photo?.Select(x => x.FileId).ToArray();
//         var newWish = new UserWish()
//         {
//             UserId = msg.Chat.Id,
//             Message = message,
//             Images = images
//         };
//         _db.UserWishes.Insert(newWish);
//         return Task.CompletedTask;
//     }
// }
