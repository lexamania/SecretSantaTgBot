using SecretSanta.TelegramBot.Enums;
using SecretSanta.TelegramBot.Models;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

using Telegram.Bot.Types;

namespace SecretSanta.TelegramBot.Extensions;

public static class EnterValueStateExtensions
{
    extension(EnterValueState state)
    {
        public static EnterValueState NewTitle(
            IMessageState parrent,
            ServicesContext services,
            EnterValueState.ProcessHandler handler
        ) => new(
            parrent,
            services,
            StatesList.EnterTitle,
            StateArgsList.Title,
            BotContent.Messages.EnterTitle,
            handler
        );

        public static EnterValueState NewDiscription(
            IMessageState parrent,
            ServicesContext services,
            EnterValueState.ProcessHandler handler
        ) => new(
            parrent,
            services,
            StatesList.EnterDescription,
            StateArgsList.Description,
            BotContent.Messages.EnterDescription,
            handler
        );

        public static EnterValueState NewUsername(
            IMessageState parrent,
            ServicesContext services,
            EnterValueState.ProcessHandler handler
        ) => new(
            parrent,
            services,
            StatesList.EnterUserName,
            StateArgsList.Name,
            BotContent.Messages.EnterName,
            handler
        );

        public static EnterValueState NewRoomAlias(
            IMessageState parrent,
            ServicesContext services,
            EnterValueState.ProcessHandler handler,
            EnterValueState.ButtonsGenerator buttons
        ) => new(
            parrent,
            services,
            StatesList.EnterRoomAlias,
            StateArgsList.RoomAlias,
            BotContent.Messages.ChooseRoom,
            handler,
            buttons
        );
    }
}
