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
    /// <param name="sender"></param>
    /// <returns></returns>
    protected IObservable<TEventMessage> GetEventStream<TEventMessage>(IEventActorRef<TEventMessage> sender)
        where TEventMessage : ActorEventMessage
        => _subscriptionManager.GetEventStream(sender, UnsafeReceive, () => Sender);
}