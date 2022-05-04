using System.Reactive.Linq;

using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Exceptions;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

public class DemoEvent
{
    public record Commands : FrameworkMessages.Command;

    public record Events : FrameworkMessages.Event
    {
        public record SomeEvent : Events;

        public record AnotherEvent : Events;
    }

    public class Actor : EventActor<Commands, Events>
    {
    }
}

public class DemoRelayActor
{
    public class Actor : EventRelayActor<DemoEvent.Commands, DemoEvent.Events>
    {
        public Actor()
        {
            AddRelay<DemoEvent.Events.SomeEvent>(x =>
                Context.ActorOf<DemoEvent.Actor, DemoEvent.Commands, DemoEvent.Events>());
            AddRelay<DemoEvent.Events.AnotherEvent>(x =>
                Context.ActorOf<DemoEvent.Actor, DemoEvent.Commands, DemoEvent.Events>());
        }
    }
}

public class SubscribingTest
{
    public record Commands : FrameworkMessages.Command
    {
    }

    public class Actor : SubscribingActor<Commands>
    {
        public Actor()
        {
            IEventActorRef<DemoEvent.Events> sender = null!;

            var articles = new[] { 100, 200, 300 };
            var articleIdStream = articles.ToObservable();
            var articlesStream = Observable.(articles, x => x);

            GetEventStream(sender, (DemoEvent.Events.SomeEvent x) => true);

            IObservable<DemoEvent.Events.SomeEvent> stream;
        }
    }
}

public class DemoSubscriber
{
}

public abstract class EventRelayActor<TCommandBase, TEventBase>
    : ReceiveActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    private readonly HashSet<SubscriptionHandler> _handler = new();

    // tracks all relayed subscriptions to reconnect them once a child-actor crashes
    private readonly Dictionary<Type, HashSet<IActorRef>> _subscriptions = new();

    protected EventRelayActor()
    {
        UnsafeReceive<FrameworkMessages.SubscriptionUpdate>(
            msg =>
            {
                var subs = _subscriptions.GetOrAdd(msg.MessageType);
                var subHandler = _handler.Where(x => x.EventType.IsAssignableFrom(msg.MessageType))
                                     .MaxBy(x => x.EventType.GetInheritanceDepth())
                                 ?? throw new UnhandledMessageException(msg);

                switch (msg)
                {
                    case FrameworkMessages.Subscribe subscription:
                        subs.Add(Sender);
                        var source = subHandler.GetEventSource(subscription);
                        source.Tell(subscription.CloneWithReceiver(Sender));
                        break;
                    case FrameworkMessages.Unsubscribe unSubscription:
                        subs.Remove(Sender);
                        var source = subHandler.GetEventSource(unSubscription);
                        source.Tell(unSubscription.CloneWithReceiver(Sender));
                        break;
                    default:
                        throw new InvalidMessageException($"{msg} is not a valid Subscription-Message");
                }
            });
    }

    protected void AddRelay<TEvent>(
        Func<FrameworkMessages.Subscribe<TEvent>, IEventActorRef<TEvent>> actorRetriever)
        where TEvent : TEventBase
    {
        if (_handler.Any(x => x.EventType == typeof(TEvent)))
            throw new InvalidSetupException($"The Event {typeof(TEvent)} has already been handled in type {GetType()}");
        _handler.Add(new(typeof(TEvent),
            eventHandler => actorRetriever((FrameworkMessages.Subscribe<TEvent>)eventHandler).Native));
    }

    private record SubscriptionHandler(
        // the type of events are handled by this source
        Type EventType,
        // returns the actor which sends the events to the Consumer
        Func<FrameworkMessages.Subscribe, IActorRef> GetEventSource);
}

/// <summary>
///     sends events
///     does not receive events, if it does, use <see cref="SubscribingEventActor{TMessageBase,TEventMessageBase}" />
///     instead.
/// </summary>
/// <typeparam name="TCommandBase"></typeparam>
/// <typeparam name="TEventBase"></typeparam>
public abstract class EventActor<TCommandBase, TEventBase> : ReceiveActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    private readonly Dictionary<Type, HashSet<IActorRef>> _subscriptions = new();

    protected EventActor()
    {
        UnsafeReceive<FrameworkMessages.SubscriptionUpdate>(
            msg =>
            {
                var subs = _subscriptions.GetOrAdd(msg.MessageType);
                switch (msg)
                {
                    case FrameworkMessages.Subscribe:
                        subs.Add(msg.Receiver ?? Sender);
                        break;
                    case FrameworkMessages.Unsubscribe:
                        subs.Remove(msg.Receiver ?? Sender);
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