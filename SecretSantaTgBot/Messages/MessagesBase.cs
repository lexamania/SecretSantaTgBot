namespace SecretSantaTgBot.Messages;

public abstract class MessagesBase
{
    public abstract string CommandError { get; }
    public abstract string CommandBotMenu { get; }
    public abstract string CommandHelp { get; }
    public abstract string CommandParticipate { get; }
    public abstract string CommandStopParticipate { get; }
    public abstract string CommandShowParticipants { get; }
    public abstract string CommandShowTarget { get; }
    public abstract string CommandShowTargetWishes { get; }
    public abstract string CommandStartWishes { get; }
    public abstract string CommandStopWishes { get; }
    public abstract string CommandClearWishes { get; }
    public abstract string CommandShowMyWishes { get; }

    public abstract string CommandCreateRoom { get; }
    public abstract string CommandJoinRoom { get; }
    public abstract string CommandSelectRoom { get; }
    public abstract string CommandDeleteRoom { get; }
    public abstract string CommandShowRooms { get; }

    public abstract string ZeroRooms { get; }
    public abstract string ChooseRoom { get; }
    public abstract string RoomCreationEnterTitle { get; }
    public abstract string RoomCreationEnterDescription { get; }
    public abstract string EnterRealName { get; }
    public abstract string RoomDoesntExist { get; }
    public abstract string RoomCreated { get; }
    public abstract string RoomDeleted { get; }
    public abstract string RoomsList { get; }

    public abstract string UserNewParticipation { get; }
    public abstract string UserParticipationEnd { get; }
    public abstract string EnteredNameError { get; }
    public abstract string UserTakeParticipation { get; }
    public abstract string UserCantCancelParticipation { get; }
    public abstract string UserDontTakeParticipation { get; }
    public abstract string UserRemovedFromParticipation { get; }
    public abstract string EmptyParticipants { get; }
    public abstract string ParticipantsList { get; }

    public abstract string UserTarget { get; }
    public abstract string TargetWishesList { get; }
    public abstract string TargetHaveZeroWishes { get; }
    public abstract string UserWishesCleared { get; }
    public abstract string UserHaveZeroWishes { get; }
    public abstract string UserWishesList { get; }
    public abstract string UserStartWishes { get; }
    public abstract string UserStopWishes { get; }

    public abstract string SecretSantaStillOffline { get; }
    public abstract string StartSanta { get; }
}
