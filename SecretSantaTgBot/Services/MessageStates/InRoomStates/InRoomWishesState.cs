using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Models;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomWishesState(MessageBrokerService csm, string parentTitle)
    : SimpleMessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_wishes";

    protected override string Message => Msgs.UserStartWishes;

    public override async Task<bool> OnMessage(Message msg, UserTg user)
    {
        var room = user.SelectedRoom!;
        var me = room.Users.First(x => x.Id == user.Id);

        if (MessageParser.IsCommand(msg, out var command, out var args))
        {
            await CallRequiredCommand(command!, msg, user, args);
            return true;
        }

        if (MessageParser.IsMessage(msg, out var message))
        {
            me.Wishes.Add(new() { Message = message, });
            DB.Rooms.Update(room);
            await NotifyService.SendMessage(user.Id, Msgs.UserWishAdded);
            return true;
        }

        if (MessageParser.IsImage(msg, out var capture, out var image))
        {
            var lastWish = me.Wishes.LastOrDefault();
            if (msg.Caption is null && lastWish is not null && lastWish.Images.Count > 0)
            {
                lastWish.Images.Add(image!.FileId);
            }
            else
            {
                var wish = new UserWish()
                {
                    Message = msg.Caption,
                    Images = [image!.FileId]
                };
                me.Wishes.Add(wish);
            }

            DB.Rooms.Update(room);
            await NotifyService.SendMessage(user.Id, Msgs.UserWishAdded);
            return true;
        }

        await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
        return true;
    }
}
