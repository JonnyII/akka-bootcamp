using System.Linq.Expressions;

using Akka.Actor;
using Akka.Util;

namespace CargoSupport.Akka.Typed;

public abstract record ActorMessage;

public interface ICanTell<TMessage>
    where TMessage : ActorMessage
{
    void Tell(TMessage message, IActorRef<TMessage>? sender = null);
}

public interface IActorRef<TActorMessageBase> : ICanTell<TActorMessageBase>
    where TActorMessageBase : ActorMessage
{
    ISurrogate ToSurrogate(ActorSystem system);
    ActorPath Path { get; }
    internal IActorRef Source { get; }

    void Tell(SystemMessages.SystemMessage message, IActorRef<TActorMessageBase>? sender = null);
}

public class ActorRefWrapper<TActorMessageBase> : IActorRef<TActorMessageBase>
    where TActorMessageBase : ActorMessage
{
    private readonly IActorRef _source;
    IActorRef IActorRef<TActorMessageBase>.Source => _source;


    internal ActorRefWrapper(IActorRef source)
    {
        _source = source;
    }

    public void Tell(TActorMessageBase message, IActorRef<TActorMessageBase>? sender = null)
        => _source.Tell(message, sender?.Source ?? ActorCell.GetCurrentSelfOrNoSender());// 2nd parameter extracted from ActorRefImplicitSenderExtensions

    public void Tell(SystemMessages.SystemMessage message, IActorRef<TActorMessageBase>? sender = null)
        => _source.Tell(message.GetNative());

    public bool Equals(IActorRef? other)
        => _source.Equals(other);

    public int CompareTo(IActorRef? other)
        => _source.CompareTo(other);

    public ISurrogate ToSurrogate(ActorSystem system)
        => _source.ToSurrogate(system);

    public int CompareTo(object? obj)
        => _source.CompareTo(obj);

    public ActorPath Path
        => _source.Path;
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
        => _actorSelection.Tell(message, sender?.Source);

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