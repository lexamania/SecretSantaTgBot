using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomWishesState(ServiceContainer container, string parentTitle)
    : SimpleMessageStateBase(container, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_wishes";

    protected override string Message => Msgs.UserStartWishes;

    public override async Task<bool> ProcessMessage(Message msg, UserEntity user)
    {
        var room = user.SelectedRoom!;
        var participant = user.GetAsParticipant(room)!;

        if (MessageParser.IsCommand(msg, out var command, out var args))
        {
            await CallRequiredCommand(command!, msg, user, args);
            return true;
        }

        if (MessageParser.IsMessage(msg, out var message))
        {
            participant.Wishes.Add(new() { Message = message, });
            Database.RoomDirectory.Update(room);
            await Notification.SendMessage(user.Id, Msgs.UserWishAdded);
            return true;
        }

        if (MessageParser.IsImage(msg, out var capture, out var image))
        {
            var lastWish = participant.Wishes.LastOrDefault();
            if (msg.Caption is null && lastWish is not null && lastWish.Images.Count > 0)
            {
                lastWish.Images.Add(image!.FileId);
            }
            else
            {
                var wish = new UserWishEntity()
                {
                    Message = msg.Caption,
                    Images = [image!.FileId]
                };
                participant.Wishes.Add(wish);
            }

            Database.RoomDirectory.Update(room);
            await Notification.SendMessage(user.Id, Msgs.UserWishAdded);
            return true;
        }

        await Notification.SendErrorCommandMessage(msg.Chat.Id, Message);
        return true;
    }
}
