using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class WishAddingState : MessageStateBase
{
    public WishAddingState(ServicesContext services)
        : base(services, StatesList.AddWishes)
    {
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        return Services.Notifications.SendMessage(
            context.User.TelegramUserId,
            context.Localization.UserStartWishes,
            MessageParser.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        var user = context.User;
        var participant = user.GetAsParticipant()!;
        var room = user.SelectedRoom!;

        switch (context.Message.Type)
        {
            case Messages.MessageType.Text:
                participant.Wishes.Add(new() { Message = context.Message.TextMsg!.Text });
                await FinalizeWishAdding(room, context);
                return true;
            case Messages.MessageType.Image:
                var imageObj = context.Message.ImageMsg!;
                var lastWish = participant.Wishes.LastOrDefault();

                if (imageObj.Text is null
                    && lastWish is not null
                    && lastWish.ImageIds.Count > 0)
                {
                    lastWish.ImageIds.Add(imageObj.ImageId);
                }
                else
                {
                    var wish = new UserWishEntity()
                    {
                        Message = imageObj.Text,
                        ImageIds = [imageObj.ImageId]
                    };
                    participant.Wishes.Add(wish);
                }

                await FinalizeWishAdding(room, context);
                return true;
            default:
                await Services.Notifications.SendErrorCommandMessage(chat.Id);
                await PrepareState(context);
                return true;
        }
    }

    private async Task FinalizeWishAdding(PartyRoomEntity room, MessageContext context)
    {
        Services.Database.Rooms.Update(room);
        await Services.Notifications.SendMessage(
            context.User.TelegramUserId,
            context.Localization.UserWishAdded);
        await PrepareState(context);
    }
}
