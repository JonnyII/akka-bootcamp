using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public enum ActorReceiverFallbackMode
{
    /// <summary>
    /// Throws an <see cref="UnhandledMessageException"/> if the Actor receives a message which is not of it's designated message type of type <see cref="IMultiActorMessage"/>.
    /// </summary>
    Throw,
    Ignore
}

public interface IActor<TMessageBase>
    where TMessageBase : ActorMessage
{

}

public abstract class Actor<TThis> : UntypedActor
    where TThis : Actor<TThis>
{
    internal Actor() { }

    public static string DefaultName
        => ActorHelper.GetDefaultName<TThis>();
}

public abstract class Actor<TThis, TMessageBase> : Actor<TThis>, IActor<TMessageBase>
    where TMessageBase : ActorMessage
    where TThis : Actor<TThis, TMessageBase>
{
    private readonly ActorReceiverFallbackMode _receiverFallbackMode;

    protected Actor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw)
    {
        _receiverFallbackMode = receiverFallbackMode;
    }
    protected new IActorRef<TMessageBase> Self => base.Self.Receives<TMessageBase>();
    private void Receiver(object rawMessage, Action<TMessageBase> handler, Action<IMultiActorMessage>? genericReceiver)
    {
        switch (rawMessage)
        {
            case TMessageBase typedMessage:
                handler(typedMessage);
                return;
            case IMultiActorMessage genericMessage when genericReceiver is not null:
                genericReceiver(genericMessage);
                return;
        }

        switch (_receiverFallbackMode)
        {
            case ActorReceiverFallbackMode.Throw:
                throw new UnhandledMessageException(rawMessage);
            case ActorReceiverFallbackMode.Ignore:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void OnReceive(object rawMessage)
        => Receiver(rawMessage, OnReceive, OnReceive);
    protected virtual void OnReceive(IMultiActorMessage multiActorMessage) { }
    public void Become(Action<TMessageBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    public void BecomeStacked(Action<TMessageBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    protected abstract void OnReceive(TMessageBase message);
}
public abstract class Actor<TThis, TMessageBase, TParent, TParentMessageBase>
    : Actor<TThis, TMessageBase>
    where TThis : Actor<TThis, TMessageBase>
    where TMessageBase : ActorMessage
    where TParent : Actor<TParent, TParentMessageBase>
    where TParentMessageBase : ActorMessage
{
    // impossible, since the child can send messages too
    //protected new IActorRef<TParent, TParentMessageBase> Sender
    //    => base.Sender.Receives<TParent, TParentMessageBase>();
    protected Actor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw) : base(receiverFallbackMode)
    {
    }
}