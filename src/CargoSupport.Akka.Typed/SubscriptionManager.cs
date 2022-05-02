using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Akka.Actor;

namespace CargoSupport.Akka.Typed;

/// <summary>
/// handles event subscription logic for actors
/// </summary>
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