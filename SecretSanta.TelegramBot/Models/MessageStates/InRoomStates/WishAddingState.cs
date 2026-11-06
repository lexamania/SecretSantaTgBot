using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Extensions;
using SecretSanta.TelegramBot.Models.Commands.General;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Storage.Entities;
using SecretSanta.TelegramBot.Utils;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Models.MessageStates.InRoomStates;

public class WishAddingState : MessageStateBase
{
    public WishAddingState(
        IMessageState parrent,
        ServicesContext services)
        : base(StatesList.AddWishes, parrent, services)
    {
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    public override Task SendWelcomeMessage(Chat chat, MessageContext context)
    {
        return Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.UserAddWishes,
            ButtonUtil.GetCommandsAsButtons(this)!);
    }

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.CustomHandler.HandleCommand(this, chat, context))
            return true;

        var user = context.User;
        var participant = user.GetAsParticipant()!;
        var room = user.SelectedRoom!;

        switch (context.Message.Type)
        {
            case Messages.MessageType.Text:
                participant.Wishes.Add(new() { Message = context.Message.TextMsg!.Text });
                await FinalizeWishAdding(room, context);
                break;
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
                break;
            default:
                await Services.Notifications.SendWrongText(chat.Id);
                break;
        }

        return true;
    }

    private async Task FinalizeWishAdding(PartyRoomEntity room, MessageContext context)
    {
        Services.Database.Rooms.Update(room);
        await Services.Notifications.SendStagedMessageByKey(
            context.User.TelegramUserId,
            BotContent.Messages.UserWishIsAdded);
    }
}
