using Akka.Actor;

namespace CargoSupport.Akka.Typed;

/// <summary>
/// sends events
/// does not receive events, if it does, use <see cref="SubscribingEventActor{TMessageBase,TEventMessageBase}"/> instead.
/// </summary>
/// <typeparam name="TMessageBase"></typeparam>
/// <typeparam name="TEventMessages"></typeparam>
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