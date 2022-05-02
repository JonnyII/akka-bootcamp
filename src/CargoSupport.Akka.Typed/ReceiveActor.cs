using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public abstract class ReceiveActor<TMessageBase> : ReceiveActor, IActor<TMessageBase>
    where TMessageBase : ActorMessage
{
    private readonly ActorReceiverFallbackMode _receiverFallbackMode;


    #region Receive
    /// <summary>
    /// <inheritdoc cref="ReceiveActor.Receive{T}(Action{T},Predicate{T})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    protected new void Receive<T>(Predicate<T> filter, Action<T> action)
        where T : TMessageBase
        => base.Receive(filter, action);

    internal void UnsafeReceive<T>(Predicate<T> filter, Action<T> action)
        => base.Receive(filter, action);
    internal void UnsafeReceive<T>(Action<T> action)
        => base.Receive(action);
    internal void UnsafeReceive<T>(Func<T, bool> action)
        => base.Receive(action);

    /// <summary>
    /// <inheritdoc cref="ReceiveActor.Receive{T}(Func{T,bool})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected new void Receive<T>(Func<T, bool> action)
        where T : TMessageBase
        => base.Receive(action);

    /// <summary>
    /// <inheritdoc cref="ReceiveActor.Receive{T}(Func{T,bool})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    protected void Receive<T>(Action<T> action)
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
        => base.Receive(action, filter);

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

public abstract class EventActor<TMessageBase, TEventMessages> : ReceiveActor<TMessageBase>
    where TMessageBase : ActorMessage
    where TEventMessages : ActorEventMessage
{
    private readonly Dictionary<Type, HashSet<IActorRef>> _subscriptions = new();

    protected void ReceiveEvent<TEvent>(Action<TEvent> handler)
        where TEvent : ActorEventMessage
        => UnsafeReceive(handler);
    protected void ReceiveEvent<TEvent>(Predicate<TEvent> predicate, Action<TEvent> handler)
        where TEvent : ActorEventMessage
        => UnsafeReceive(predicate, handler);
    protected void ReceiveEvent<TEvent>(Func<TEvent, bool> handler)
        where TEvent : ActorEventMessage
        => UnsafeReceive(handler);
    protected EventActor()
    {
        this.UnsafeReceive<SubscriptionMessage>(
            msg =>
            {
                var subs = _subscriptions.GetOrAdd(msg.MessageType);
                switch (msg)
                {
                    case Subscribe:
                        subs.Add(Sender);
                        break;
                    case Unsubscribe:
                        subs.Remove(Sender);
                        break;
                    default:
                        throw new InvalidMessageException($"{msg} is not a valid Subscription-Message");
                }
            });

    }

    protected void PublishEvent<TEvent>(TEvent eventMessage)
        where TEvent : TEventMessages
    {
        var subs = _subscriptions.GetValueOrDefault(eventMessage.GetType());
        if (subs is null)
            return;
        foreach (var sub in subs)
            sub.Tell(eventMessage);
    }
}

public static class Helper
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue> valueFactory)
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        value = valueFactory();
        dictionary.Add(key, value);

        return value;
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
        => dictionary.GetOrAdd(key, () => new());
}

internal class SubscriptionManager : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly Dictionary<Type, Subject<ActorEventMessage>> _relayRegistry = new();
    public void Dispose()
    {
        _disposables.Dispose();
        _relayRegistry.Select(x => x.Value).ToList()
            .ForEach(o => o.OnCompleted());
    }

    /// <summary>
    /// initializes the subscription
    /// sends subscribe
    /// add receive handler
    /// provides observable which
    /// - sends unsubscribe to sender once all the subject is no longer subscribed to
    /// - applies Publish and refCount to the underlying observable
    /// multiple message of the same type are handled by comparing the <see cref="sender"/> with the result of <see cref="getSender"/>.
    /// this should enable us to handle the same message from different senders without handling the events for the wrong actors
    /// </summary>
    /// <typeparam name="TEventMessage"></typeparam>
    /// <param name="sender">the sender of the observable updates</param>
    /// <param name="addReceiveHandler">used to register a handler for the given event on the <see cref="receiver"/></param>
    /// <param name="getSender">used to get the reference of the sender </param>
    /// <param name="filter">ignore all messages which do not satisfy this criteria</param>
    /// <returns></returns>
    internal IObservable<TEventMessage> GetEventStream<TEventMessage>(
        IEventActorRef<TEventMessage> sender,
        Action<Predicate<TEventMessage>, Action<TEventMessage>> addReceiveHandler,
        Func<IActorRef> getSender,
        Predicate<TEventMessage>? filter = null)
        where TEventMessage : ActorEventMessage
    {
        return Observable.Create((IObserver<TEventMessage> observer) =>
        {
            addReceiveHandler(
                //ignore all messages if the path does not match the original sender path
                message => sender.Path == getSender().Path && filter?.Invoke(message) is not false,
                observer.OnNext);
            return sender.ListenTo<TEventMessage>();
        }).Publish().RefCount();
    }
}
/// <summary>
/// subscribes to events from other actors
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public class SubscribingActor<TMessage> : ReceiveActor<TMessage>
    where TMessage : ActorMessage
{
    private readonly SubscriptionManager _subscriptionManager;

    public SubscribingActor()
    {
        _subscriptionManager = new();

    }
    /// <summary>
    /// creates an observable which 
    /// </summary>
    /// <typeparam name="TEventMessage"></typeparam>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected IObservable<TEventMessage> GetEventStream<TEventMessage>(IEventActorRef<TEventMessage> sender)
        where TEventMessage : ActorEventMessage
        => _subscriptionManager.GetEventStream(sender, UnsafeReceive, () => Sender);
}

/// <summary>
/// subscribes to events from other actors
/// sends events itself
/// </summary>
public class SubscribingEventActor
{

}