using SecretSanta.Shared.Enums;
using SecretSanta.TelegramBot.Models.MessageStates.Base;
using SecretSanta.TelegramBot.Models.MessageStates.Common;

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
            services.Localizer.Messages.EnterTitle,
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
            services.Localizer.Messages.EnterDescription,
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
            services.Localizer.Messages.EnterName,
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
            services.Localizer.Messages.ChooseRoom,
            handler,
            buttons
        );
    }
}
