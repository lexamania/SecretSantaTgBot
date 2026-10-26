using SecretSantaTgBot.Storage.Entities;

namespace SecretSantaTgBot.Models.MessageStates.Base;

public abstract class InternalMessageStateBase(
    ServicesContext services,
    string title
    ) : MessageStateBase(services, title)
{
    protected abstract Task PrepareStateInternal(MessageContext context);
    public Task PrepareState(MessageContext context)
    {
        var state = context.State.StateObject;
        state.States.Add(Title);
        Services.Database.Users.UpdateWithStateSetting(context.User, state.ToString());

        return PrepareStateInternal(context);
    }

    protected void FinishState(UserEntity user)
    {
        Services.Database.Users.UpdateWithStateClearing(user);
    }
}
