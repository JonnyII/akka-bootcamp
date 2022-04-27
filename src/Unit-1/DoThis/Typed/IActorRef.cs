using System;
using System.Linq.Expressions;

using Akka.Actor;
using Akka.Util;

namespace WinTail.Typed;

public abstract record ActorMessage;
public interface IActorRef<in TActorMessageBase>
    where TActorMessageBase : ActorMessage
{
    void Tell(TActorMessageBase message);
    ISurrogate ToSurrogate(ActorSystem system);
    ActorPath Path { get; }
}
public class ActorRefWrapper<TActorMessageBase> : IActorRef<TActorMessageBase>
    where TActorMessageBase : ActorMessage
{
    private readonly IActorRef _sourceRef;

    internal ActorRefWrapper(IActorRef sourceRef)
    {
        _sourceRef = sourceRef;
    }

    public void Tell(TActorMessageBase message)
        => _sourceRef.Tell(message, ActorCell.GetCurrentSelfOrNoSender());// 2nd parameter extracted from ActorRefImplicitSenderExtensions

    public bool Equals(IActorRef? other)
        => _sourceRef.Equals(other);

    public int CompareTo(IActorRef? other)
        => _sourceRef.CompareTo(other);

    public ISurrogate ToSurrogate(ActorSystem system)
        => _sourceRef.ToSurrogate(system);

    public int CompareTo(object? obj)
        => _sourceRef.CompareTo(obj);

    public ActorPath Path
        => _sourceRef.Path;
}

public static class ActorRefHelper
{
    /// <summary>
    /// interprets the ActorRef to be of the specified Actor Type.<br/>
    /// This does not provide runtime TypeChecks!
    /// if the actor does not have the specified type, no exception will be thrown and the messages will be send regardless, without Error!
    /// </summary>
    /// <typeparam name="TActor"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="actor"></param>
    /// <returns></returns>
    public static IActorRef<TMessage> Receives<TMessage>(this IActorRef actor)
    where TMessage : ActorMessage
        => new ActorRefWrapper<TMessage>(actor);

    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, Expression<Func<TActor>> factory, string name = null)
        where TActor : Actor<TActor, TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? typeof(TActor).Name).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, Expression<Func<TActor>> factory, string name = null)
        where TActor : Actor<TActor, TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? typeof(TActor).Name).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, string name = null)
        where TActor : Actor<TActor, TMessage>, new()
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? typeof(TActor).Name).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, string name = null)
        where TActor : Actor<TActor, TMessage>, new()
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? typeof(TActor).Name).Receives<TMessage>();

    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, TypedProps<TActor> props, string name = null)
        where TActor : Actor<TActor, TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)props, name ?? typeof(TActor).Name).Receives<TMessage>();
    public static IActorRef<TMessage> ActorOf<TActor, TMessage>(this IUntypedActorContext actorSystem, TypedProps<TActor> props, string name = null)
        where TActor : Actor<TActor, TMessage>
        where TMessage : ActorMessage
        => actorSystem.ActorOf((Props)props, name ?? typeof(TActor).Name).Receives<TMessage>();
}