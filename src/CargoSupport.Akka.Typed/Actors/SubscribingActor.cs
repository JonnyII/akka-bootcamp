namespace CargoSupport.Akka.Typed;

/// <summary>
/// subscribes to events from other actors. if it doesn't, use <see cref="ReceiveActor{TMessageBase}"/> instead
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
    /// <typeparam name="TEventMessageBase"></typeparam>
    /// <param name="sender"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    protected IObservable<TEventMessage> GetEventStream<TEventMessageBase, TEventMessage>(IEventActorRef<TEventMessageBase> sender, Predicate<TEventMessage>? filter = null)
        where TEventMessageBase : ActorEventMessage
        where TEventMessage : TEventMessageBase
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