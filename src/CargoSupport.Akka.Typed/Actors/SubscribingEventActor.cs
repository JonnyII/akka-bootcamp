using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors.CompositionComponents;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

/// <summary>
///     subscribes to events from other actors, if it doesn't, use <see cref="EventActor{TMessageBase,TEventMessages}" />
///     instead.
///     emits events if it doesn't, use <see cref="SubscribingActor{TMessage}" /> instead
/// </summary>
public class SubscribingEventActor<TCommandBase, TEventBase>
    : EventActor<TCommandBase, TEventBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    private readonly SubscriptionManager _subscriptionManager;

    public SubscribingEventActor()
    {
        _subscriptionManager = new();
    }

    /// <summary>
    ///     creates an observable which
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected IObservable<TEvent> GetEventStream<TEvent>(IEventActorRef<TEvent> sender)
        where TEvent : TEventBase
    {
        return _subscriptionManager.GetEventStream(sender, UnsafeReceive, () => Sender);
    }
}