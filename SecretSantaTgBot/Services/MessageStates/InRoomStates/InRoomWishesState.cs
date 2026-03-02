using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Services.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Services.MessageStates.InRoomStates;

public class InRoomWishesState(MessageBrokerService csm, string parentTitle)
    : SimpleMessageStateBase(csm, NameParser.JoinArgs(parentTitle, TITLE))
{
    public const string TITLE = "in_room_wishes";

    protected override string Message => Msgs.UserStartWishes;

    public override async Task<bool> OnMessage(Message msg, UserEntity user)
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
            DB.RoomDirectory.Update(room);
            await NotifyService.SendMessage(user.Id, Msgs.UserWishAdded);
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

            DB.RoomDirectory.Update(room);
            await NotifyService.SendMessage(user.Id, Msgs.UserWishAdded);
            return true;
        }

        await NotifyService.SendErrorCommandMessage(msg.Chat.Id, Message);
        return true;
    }
}
