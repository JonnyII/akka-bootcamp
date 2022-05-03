using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors.CompositionComponents;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Actors;

/// <summary>
/// subscribes to events from other actors, if it doesn't, use <see cref="EventActor{TMessageBase,TEventMessages}"/> instead.
/// emits events if it doesn't, use <see cref="SubscribingActor{TMessage}"/> instead
/// </summary>
public class SubscribingEventActor<TMessageBase, TEventMessageBase>
    : EventActor<TMessageBase, TEventMessageBase>
    where TMessageBase : FrameworkMessages.ActorCommand
    where TEventMessageBase : FrameworkMessages.ActorEventMessage
{
    private readonly SubscriptionManager _subscriptionManager;

    public SubscribingEventActor()
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
        where TEventMessage : FrameworkMessages.ActorEventMessage
        => _subscriptionManager.GetEventStream(sender, UnsafeReceive, () => Sender);

}