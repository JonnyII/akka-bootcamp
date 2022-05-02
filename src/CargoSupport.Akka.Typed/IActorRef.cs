using System.Linq.Expressions;
using System.Reactive.Disposables;

using Akka.Actor;
using Akka.Util;

namespace CargoSupport.Akka.Typed;

/// <summary>
/// incoming actor message (like a public method)
/// </summary>
public abstract record ActorMessage;

/// <summary>
/// outgoing event
/// they have to be either sealed or abstract and only the final implementation may be passed as parameter to subscribe, otherwise the handler will not be called
/// </summary>
public abstract record ActorEventMessage;

public abstract record SubscriptionMessage
{
    public abstract Type MessageType { get; }
}

public abstract record Subscribe : SubscriptionMessage;
public abstract record Unsubscribe : SubscriptionMessage;
public sealed record Subscribe<TEventMessage> : Subscribe
{
    public override Type MessageType => typeof(TEventMessage);
}
public sealed record Unsubscribe<TEventMessage> : Subscribe
{
    public override Type MessageType => typeof(TEventMessage);
}

public interface ICanTell<TMessage>
    where TMessage : ActorMessage
{
    void Tell(TMessage message, IActorRef<TMessage>? sender = null);
    internal ICanTell Native { get; }
}

public interface IActorRef<TActorMessageBase> : ICanTell<TActorMessageBase>, IPathActor
    where TActorMessageBase : ActorMessage
{
    ISurrogate ToSurrogate(ActorSystem system);
    ICanTell ICanTell<TActorMessageBase>.Native => Native;
    internal new IActorRef Native { get; }

    void Tell(SystemMessages.SystemMessage message, IActorRef<TActorMessageBase>? sender = null);
}

public interface IPathActor
{
    ActorPath Path { get; }
}
internal class ActorRefWrapper<TActorMessageBase> : IActorRef<TActorMessageBase>
    where TActorMessageBase : ActorMessage
{
    protected readonly IActorRef Source;
    IActorRef IActorRef<TActorMessageBase>.Native => Source;


    internal ActorRefWrapper(IActorRef source)
    {
        Source = source;
    }

    public void Tell(TActorMessageBase message, IActorRef<TActorMessageBase>? sender = null)
        => Source.Tell(message, sender?.Native ?? ActorCell.GetCurrentSelfOrNoSender());// 2nd parameter extracted from ActorRefImplicitSenderExtensions


    public void Tell(SystemMessages.SystemMessage message, IActorRef<TActorMessageBase>? sender = null)
        => Source.Tell(message.GetNative());

    public bool Equals(IActorRef? other)
        => Source.Equals(other);

    public int CompareTo(IActorRef? other)
        => Source.CompareTo(other);

    public ISurrogate ToSurrogate(ActorSystem system)
        => Source.ToSurrogate(system);

    public int CompareTo(object? obj)
        => Source.CompareTo(obj);

    public ActorPath Path
        => Source.Path;
}


public interface IActorSelection<TMessage> : ICanTell<TMessage>
    where TMessage : ActorMessage
{
    IActorRef<TMessage> Anchor { get; }
    SelectionPathElement[] Path { get; }
    string PathString { get; }
    Task<IActorRef<TMessage>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null);
}

public class ActorSelectionWrapper<TMessage> : IActorSelection<TMessage>
    where TMessage : ActorMessage
{
    private readonly ActorSelection _actorSelection;

    public ActorSelectionWrapper(ActorSelection actorSelection)
    {
        _actorSelection = actorSelection;
    }

    public IActorRef<TMessage> Anchor => _actorSelection.Anchor.Receives<TMessage>();
    public SelectionPathElement[] Path => _actorSelection.Path;
    public string PathString => _actorSelection.PathString;
    public void Tell(TMessage message, IActorRef<TMessage>? sender = null)
        => _actorSelection.Tell(message, sender?.Native);

    ICanTell ICanTell<TMessage>.Native => _actorSelection;

    public Task<IActorRef<TMessage>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null)
        => _actorSelection
            .ResolveOne(timeout, ct ?? CancellationToken.None)
            .ContinueWith(actor => actor.Result.Receives<TMessage>(), ct ?? CancellationToken.None);
}

public static class ActorRefHelper
{
    /// <summary>
    /// interprets the ActorRef to be of the specified Actor Type.<br/>
    /// This does not provide runtime TypeChecks!
    /// if the actor does not have the specified type, no exception will be thrown and the messages will be send regardless, without Error!
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="actor"></param>
    /// <returns></returns>
    public static IActorRef<TMessage> Receives<TMessage>(this IActorRef actor)
    where TMessage : ActorMessage
        => new ActorRefWrapper<TMessage>(actor);

    #region ActorOf
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, Expression<Func<TActor>> factory, string? name = null)
        where TActor : ActorBase, IActor<TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, Expression<Func<TActor>> factory, string? name = null)
        where TActor : ActorBase, IActor<TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, string? name = null)
        where TActor : ActorBase, IActor<TMessage>, new()
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, string? name = null)
        where TActor : ActorBase, IActor<TMessage>, new()
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();

    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, TypedProps<TActor> props, string? name = null)
        where TActor : ActorBase, IActor<TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)props, name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, TypedProps<TActor> props, string? name = null)
        where TActor : ActorBase, IActor<TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)props, name ?? ActorHelper.GetDefaultName<TActor>()).Receives<TMessage>();
    #endregion

    public static IActorSelection<TMessage> Receives<TMessage>(this ActorSelection selection)
        where TMessage : ActorMessage
        => new ActorSelectionWrapper<TMessage>(selection);

    public static IActorSelection<TMessage> ActorSelection<TMessage>(
        this IUntypedActorContext actorSystem, string path)
    where TMessage : ActorMessage
        => actorSystem.ActorSelection(path).Receives<TMessage>();
}

public interface IEventActorRef<in TEventMessage> : IPathActor
    where TEventMessage : ActorEventMessage
{
    /// <summary>
    /// subscribes the current actor to the event stream of this actorRef
    /// sends a <see cref="Subscribe{TEvent}"/> to the referenced actor
    /// sends a <see cref="Unsubscribe{TEventMessage}"/> once the result-disposable has been disposed of
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IDisposable ListenTo<TEvent>()
        where TEvent : TEventMessage;
}
public interface IEventActorRef<TMessage, in TEventMessage> : IActorRef<TMessage>, IEventActorRef<TEventMessage>
    where TMessage : ActorMessage
    where TEventMessage : ActorEventMessage
{
}

internal class EventActorRefWrapper<TMessage, TEventMessage> : ActorRefWrapper<TMessage>, IEventActorRef<TMessage, TEventMessage>
    where TMessage : ActorMessage
    where TEventMessage : ActorEventMessage
{
    internal EventActorRefWrapper(IActorRef source) : base(source)
    {
    }

    /// <summary>
    /// dispose of the disposable to unsubscribe
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IDisposable ListenTo<TEvent>() where TEvent : TEventMessage
    {
        Source.Tell(new Subscribe<TEvent>());
        return Disposable.Create(() => Source.Tell(new Unsubscribe<TEvent>()));
    }
}
