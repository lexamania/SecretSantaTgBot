using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Extensions;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.InRoomStates;

public class WishAddingState: InternalMessageStateBase
{
    public WishAddingState(ServicesContext services)
        : base(services, StatesList.AddWishes)
    {
        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
    {
        return Services.Notifications.SendMessage(
            context.User.Id,
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

        if (context.Message.Type == Messages.MessageType.Text)
        {
            participant.Wishes.Add(new() { Message = context.Message.TextMsg!.Text });
            await FinalizeWishAdding(room, context);
            return true;
        }

        if (context.Message.Type == Messages.MessageType.Image)
        {
            var imageObj = context.Message.ImageMsg!;
            var lastWish = participant.Wishes.LastOrDefault();

            if (imageObj.Text is null 
                && lastWish is not null
                && lastWish.Images.Count > 0)
            {
                lastWish.Images.Add(imageObj.ImageId);
            }
            else
            {
                var wish = new UserWishEntity()
                {
                    Message = imageObj.Text,
                    Images = [imageObj.ImageId]
                };
                participant.Wishes.Add(wish);
            }

            await FinalizeWishAdding(room, context);
            return true;
        }
    
        await Services.Notifications.SendErrorCommandMessage(
            chat.Id,
            context.Localization.UserStartWishes);
        await PrepareState(context);
        return true;
    }

    private Task FinalizeWishAdding(PartyRoomEntity room, MessageContext context)
    {
        Services.Database.Rooms.Update(room);
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.UserWishAdded);
    }
}
