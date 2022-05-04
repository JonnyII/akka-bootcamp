using System.ComponentModel;
using Akka.Actor;
using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Exceptions;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

/// <summary>
///     receives requests from other actors
/// </summary>
/// <typeparam name="TCommandBase"></typeparam>
public abstract class ReceiveActor<TCommandBase> : ReceiveActor, IActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    private readonly ActorReceiverFallbackMode _receiverFallbackMode;


    protected ReceiveActor(ActorReceiverFallbackMode receiverFallbackMode = ActorReceiverFallbackMode.Throw)
    {
        _receiverFallbackMode = receiverFallbackMode;
    }

    protected new IActorRef<TCommandBase> Self => base.Self.HasType<TCommandBase>();

    private void Receiver(object rawMessage, Action<TCommandBase> handler, Action<IMultiActorMessage>? genericReceiver)
    {
        switch (rawMessage)
        {
            case TCommandBase typedMessage:
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

    public void Become(Action<TCommandBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
    {
        base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));
    }

    public void BecomeStacked(Action<TCommandBase> newReceiver, Action<IMultiActorMessage>? genericReceiver = null)
    {
        base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));
    }


    #region Receive

    /// <summary>
    ///     <inheritdoc cref="ReceiveActor.Receive{T}(Action{T},Predicate{T})" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    protected new void Receive<T>(Predicate<T> filter, Action<T> action)
        where T : TCommandBase
    {
        base.Receive(filter, action);
    }

    internal void UnsafeReceive<T>(Predicate<T> filter, Action<T> action)
    {
        base.Receive(filter, action);
    }

    internal void UnsafeReceive<T>(Action<T> action)
    {
        base.Receive(action);
    }

    internal void UnsafeReceive<T>(Func<T, bool> action)
    {
        base.Receive(action);
    }

    /// <summary>
    ///     <inheritdoc cref="ReceiveActor.Receive{T}(Func{T,bool})" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected new void Receive<T>(Func<T, bool> action)
        where T : TCommandBase
    {
        base.Receive(action);
    }

    /// <summary>
    ///     <inheritdoc cref="ReceiveActor.Receive{T}(Func{T,bool})" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected void Receive<T>(Action<T> action)
        where T : TCommandBase
    {
        base.Receive(action);
    }

    protected void Receive<T>(Action action)
        where T : TCommandBase
    {
        base.Receive<T>(_ => action());
    }


    #region obsolete overrides

    //overriding base members 
    /// <summary>
    ///     <inheritdoc cref="ReceiveActor.Receive(Type, Predicate{object}, Action{object})" />
    /// </summary>
    /// <param name="type"></param>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    [Obsolete("use the generic overload instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void Receive(Type type, Predicate<object> filter, Action<object> action)
    {
        base.Receive(type, filter, action);
    }

    /// <summary>
    ///     <inheritdoc cref="ReceiveActor.Receive{T}(Action{T},Predicate{T})" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="filter"></param>
    [Obsolete("filter first, than take action. (use the other overload with switched parameter order)")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected new void Receive<T>(Action<T> action, Predicate<T> filter)
        where T : TCommandBase
    {
        base.Receive(action, filter);
    }

    [Obsolete($"use {nameof(Receive)} instead")]
    protected new void ReceiveAny(Action<object> action)
    {
        base.ReceiveAny(action);
    }

    #endregion

    #endregion

    #region ReceiveAsync

    protected new void ReceiveAsync<T>(Predicate<T> filter, Func<T, Task> action)
        where T : TCommandBase
    {
        base.ReceiveAsync(filter, action);
    }

    #region deprecated

    [Obsolete($"use {nameof(ReceiveAsync)} instead")]
    protected new void ReceiveAnyAsync(Func<object, Task> action)
    {
        base.ReceiveAnyAsync(action);
    }

    #endregion

    #endregion
}