using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Exceptions;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

public enum ActorReceiverFallbackMode
{
    /// <summary>
    /// Throws an <see cref="UnhandledMessageException"/> if the Actor receives a message which is not of it's designated message type of type <see cref="IMultiActorMessage"/>.
    /// </summary>
    Throw,
    Ignore
}

public interface IActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{

}
[Obsolete("use ReceiveActor instead")]
public abstract class Actor<TCommandBase> : UntypedActor, IActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    private readonly ActorReceiverFallbackMode _receiverFallbackMode;

    protected Actor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw)
    {
        _receiverFallbackMode = receiverFallbackMode;
    }
    protected new IActorRef<TCommandBase> Self => base.Self.Receives<TCommandBase>();
    private void Receiver(object rawMessage, Action<TCommandBase> handler, Action<IMultiActorMessage>? genericReceiver)
    {
        switch (rawMessage)
        {
            case TCommandBase typedCommand:
                handler(typedCommand);
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
    public void Become(Action<TCommandBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    public void BecomeStacked(Action<TCommandBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    protected abstract void OnReceive(TCommandBase message);
}
[Obsolete("use ReceiveActor instead")]
public abstract class Actor<TCommandBase, TParentCommandBase>
    : Actor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
    where TParentCommandBase : FrameworkMessages.Command
{
    // impossible, since the child can send messages too
    //protected new IActorRef<TParent, TParentMessageBase> Sender
    //    => base.Sender.Receives<TParent, TParentMessageBase>();
    protected Actor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw) : base(receiverFallbackMode)
    {
    }
}

