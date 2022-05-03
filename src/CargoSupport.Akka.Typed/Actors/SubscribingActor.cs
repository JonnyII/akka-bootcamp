using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors.CompositionComponents;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

/// <summary>
/// subscribes to events from other actors. if it doesn't, use <see cref="ReceiveActor{TMessageBase}"/> instead
/// </summary>
/// <typeparam name="TCommandBase"></typeparam>
public class SubscribingActor<TCommandBase> : ReceiveActor<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    private readonly SubscriptionManager _subscriptionManager;

    public SubscribingActor()
    {
        _subscriptionManager = new();

    }

    /// <summary>
    /// creates an observable which 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventBase"></typeparam>
    /// <param name="sender"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    protected IObservable<TEvent> GetEventStream<TEventBase, TEvent>(IEventActorRef<TEventBase> sender, Predicate<TEvent>? filter = null)
        where TEventBase : FrameworkMessages.Event
        where TEvent : TEventBase
        => _subscriptionManager.GetEventStream(sender, UnsafeReceive, () => Sender, filter);
}

/*
 * event nutzen pub/sub pattern
 *                              Receives Requests  |  emits events  | subscribes to events
 * Receive Actor                    X              |                |    
 * Event Actor                      X              |      X         |     
 * Subscribing Actor                X              |                |     X
 * Subscribing Event Actor          X              |      X         |     X
 *
 */