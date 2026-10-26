using SecretSantaTgBot.Enums;
using SecretSantaTgBot.Models.Commands.General;
using SecretSantaTgBot.Models.MessageStates.Base;
using SecretSantaTgBot.Storage.Entities;
using SecretSantaTgBot.Utils;

using Telegram.Bot.Types;

namespace SecretSantaTgBot.Models.MessageStates.DefaultStates;

public class RoomCreationState : InternalMessageStateBase
{
    public RoomCreationState(ServicesContext services)
        : base(services, StatesList.RoomCreation)
    {
        Commands[BotCommandsList.Help] = new HelpBotCommand(services, this);
        Commands[BotCommandsList.Stop] = new StopBotCommand(services);
    }

    protected override Task PrepareStateInternal(MessageContext context)
        => Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.RoomCreationEnterTitle,
            MessageParser.GetCommandsAsButtons(this)!);

    public override async Task<bool> ProcessMessage(Chat chat, MessageContext context)
    {
        if (await Services.ProcessHandler.HandleCommand(this, chat, context))
            return true;

        var firstStep = !context.State.StateObject.Arguments
            .TryGetValue(StateArgsList.Title, out var title);

        var enterMessage = firstStep
            ? context.Localization.RoomCreationEnterTitle
            : context.Localization.RoomCreationEnterDescription;

        if (context.Message.Type != Messages.MessageType.Text)
        {
            await Services.Notifications.SendErrorCommandMessage(chat.Id, enterMessage);
            return true;
        }

        var task = firstStep
            ? SaveTitle(context)
            : CreateRoom(
                context.User,
                title!,
                context.Message.TextMsg!.Text);

        await task;
        return true;
    }

    private Task SaveTitle(MessageContext context)
    {
        var stateObj = context.State.StateObject;
        stateObj.Arguments[StateArgsList.Title] = context.Message.TextMsg!.Text;

        Services.Database.Users.UpdateWithStateSetting(context.User, stateObj.ToString());
        return Services.Notifications.SendMessage(
            context.User.Id,
            context.Localization.RoomCreationEnterDescription);
    }

    private Task CreateRoom(UserEntity user, string title, string description)
    {
        var room = Services.Database.Rooms.Create(user, title, description);

        user.AvailableRooms.Add(room);

        FinishState(user);

        var message = Services.MessageBuilder.BuildCreateRoomMessage(room.Id.ToString());
        return Services.Notifications.SendMessage(user.Id, message);
    }
}
