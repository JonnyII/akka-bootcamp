using System;

using Akka.Actor;
using Akka.Util;

namespace WinTail.Typed;

public interface IActorRef<TActor, TActorMessageBase>
    where TActor : Actor<TActor, TActorMessageBase>
{
}

public class ActorRefWrapper<TActor, TActorMessageBase> : IActorRef<TActor, TActorMessageBase>
    where TActor : Actor<TActor, TActorMessageBase>
{
    private readonly IActorRef _sourceRef;

    internal ActorRefWrapper(IActorRef sourceRef)
    {
        _sourceRef = sourceRef;
    }

    public void Tell(TActorMessageBase message, IActorRef sender)
        => _sourceRef.Tell(message, sender);

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
public abstract class Actor<TThis> : UntypedActor
    where TThis : Actor<TThis>
{
    internal Actor() { }
}
public abstract class Actor<TThis, TMessageBase> : Actor<TThis>
    where TThis : Actor<TThis, TMessageBase>
{
    private static void Receiver(object rawMessage, Action<TMessageBase> handler)
    {
        if (rawMessage is TMessageBase typedMessage)
            handler(typedMessage);
    }

    protected override void OnReceive(object rawMessage)
        => Receiver(rawMessage, OnReceive);

    public void Become(Action<TMessageBase> newReceiver)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver));
    public void BecomeStacked(Action<TMessageBase> newReceiver)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver));

    protected abstract void OnReceive(TMessageBase message);
}

public class TypedProps
{
    public static TypedProps<TActor> Create<TActor>(Func<TActor> factory, SupervisorStrategy supervisorStrategy = null)
          where TActor : Actor<TActor>
          => new(Props.Create(() => factory(), supervisorStrategy));
    public static TypedProps<TActor> Create<TActor>(SupervisorStrategy supervisorStrategy = null)
        where TActor : Actor<TActor>, new()
        => new(Props.Create(() => new TActor(), supervisorStrategy));
}
public class TypedProps<TActor>
    where TActor : Actor<TActor>
{
    private readonly Props _source;

    internal TypedProps(Props source)
    {
        _source = source;
    }

    public static explicit operator Props(TypedProps<TActor> props)
        => props._source;
}

public static class TypedPropsHelper
{
    public static IActorRef<TActor, TMessage> Of<TActor, TMessage>(this IActorRef actor)
        where TActor : Actor<TActor, TMessage>
        => new ActorRefWrapper<TActor, TMessage>(actor);

    public static IActorRef<TActor, TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, TypedProps<TActor> props, string name = null)
        where TActor : Actor<TActor, TMessage>
        => actorSystem.ActorOf((Props)props, name ?? typeof(TActor).Name).Of<TActor, TMessage>();
}

interface ITestActorCommand { }
class TestActor : Actor<TestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message)
    {

    }
}
class DependentTestActor : Actor<TestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message)
    {

    }

    public DependentTestActor(IActorRef<TestActor, ITestActorCommand> testActor)
    {

    }
}

