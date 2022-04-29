using System.ComponentModel;

using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public abstract class ReceiveActor<TThis, TMessageBase> : ReceiveActor
    where TMessageBase : ActorMessage
    where TThis : ReceiveActor<TThis, TMessageBase>
{
    private readonly ActorReceiverFallbackMode _receiverFallbackMode;

    #region Receive

    protected new void Receive<T>(Predicate<T> filter, Action<T> action)
        where T : TMessageBase
        => base.Receive(filter, action);

    protected new void Receive<T>(Func<T, bool> action)
        where T : TMessageBase
        => base.Receive(action);

    #region obsolete overrides
    //overriding base members 
    /// <summary>
    /// <inheritdoc cref="ReceiveActor.Receive(Type, Predicate{object}, Action{object})"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    [Obsolete($"use the generic overload instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void Receive(Type type, Predicate<object> filter, Action<object> action)
        => base.Receive(type, filter, action);

    /// <summary>
    /// <inheritdoc cref="ReceiveActor.Receive{T}(Action{T},Predicate{T})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="filter"></param>
    [Obsolete($"filter first, than take action. (use the other overload with switched parameter order)")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void Receive<T>(Action<T> action, Predicate<T> filter)
        where T : TMessageBase
        => base.Receive(filter, action);

    [Obsolete($"use {nameof(Receive)} instead")]
    protected new void ReceiveAny(Action<object> action)
        => base.ReceiveAny(action);

    #endregion

    #endregion

    #region ReceiveAsync

    protected new void ReceiveAsync<T>(Predicate<T> filter, Func<T, Task> action)
        where T : TMessageBase
        => base.ReceiveAsync(filter, action);

    #region deprecated
    [Obsolete($"use {nameof(ReceiveAsync)} instead")]
    protected new void ReceiveAnyAsync(Func<object, Task> action)
        => base.ReceiveAnyAsync(action);
    #endregion

    #endregion



    protected ReceiveActor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw)
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
    public void Become(Action<TMessageBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    public void BecomeStacked(Action<TMessageBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

}
