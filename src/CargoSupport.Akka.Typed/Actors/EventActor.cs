using Akka.Actor;

using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

/// <summary>
/// sends events
/// does not receive events, if it does, use <see cref="SubscribingEventActor{TMessageBase,TEventMessageBase}"/> instead.
/// </summary>
/// <typeparam name="TCommandBase"></typeparam>
/// <typeparam name="TEventBase"></typeparam>
public abstract class EventActor<TCommandBase, TEventBase> : ReceiveActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    private readonly Dictionary<Type, HashSet<IActorRef>> _subscriptions = new();

    protected void ReceiveEvent<TEvent>(Action<TEvent> handler)
        where TEvent : FrameworkMessages.Event
        => UnsafeReceive(handler);
    protected void ReceiveEvent<TEvent>(Predicate<TEvent> predicate, Action<TEvent> handler)
        where TEvent : FrameworkMessages.Event
        => UnsafeReceive(predicate, handler);
    protected void ReceiveEvent<TEvent>(Func<TEvent, bool> handler)
        where TEvent : FrameworkMessages.Event
        => UnsafeReceive(handler);
    protected EventActor()
    {
        this.UnsafeReceive<FrameworkMessages.SubscriptionUpdate>(
            msg =>
            {
                var subs = _subscriptions.GetOrAdd(msg.MessageType);
                switch (msg)
                {
                    case FrameworkMessages.Subscribe:
                        subs.Add(Sender);
                        break;
                    case FrameworkMessages.Unsubscribe:
                        subs.Remove(Sender);
                        break;
                    default:
                        throw new InvalidMessageException($"{msg} is not a valid Subscription-Message");
                }
            });

    }

    protected void PublishEvent<TEvent>(TEvent eventMessage)
        where TEvent : TEventBase
    {
        var subs = _subscriptions.GetValueOrDefault(eventMessage.GetType());
        if (subs is null)
            return;
        foreach (var sub in subs)
            sub.Tell(eventMessage);
    }
}