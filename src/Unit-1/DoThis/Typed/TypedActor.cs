using System;

using Akka.Actor;
using Akka.Util;

namespace WinTail.Typed;

public interface IActorRef<TActor> : IActorRef
    where TActor : Actor<TActor>
{
}

public class ActorRefWrapper<TActor> : IActorRef<TActor>
    where TActor : Actor<TActor>
{
    private readonly IActorRef _sourceRef;

    internal ActorRefWrapper(IActorRef sourceRef)
    {
        _sourceRef = sourceRef;
    }

    public void Tell(object message, IActorRef sender)
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
}
public abstract class Actor<TThis, TMessageBase> : Actor<TThis>
    where TThis : Actor<TThis, TMessageBase>
{
    protected override void OnReceive(object rawMessage)
    {
        if (rawMessage is TMessageBase typedMessage)
            OnReceive(typedMessage);
    }

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
    public static IActorRef<TActor> Of<TActor>(this IActorRef actor)
        where TActor : Actor<TActor>
        => new ActorRefWrapper<TActor>(actor);

    public static IActorRef<TActor> ActorOf<TActor>(this ActorSystem actorSystem, TypedProps<TActor> props, string name = null)
        where TActor : Actor<TActor>
        => actorSystem.ActorOf((Props)props, name ?? typeof(TActor).Name).Of<TActor>();
}

interface TestActorCommand { }
class TestActor : Actor<TestActor, TestActorCommand>
{
    protected override void OnReceive(TestActorCommand message)
    {

    }
}
class DependentTestActor : Actor<TestActor, TestActorCommand>
{
    protected override void OnReceive(TestActorCommand message)
    {

    }

    public DependentTestActor(IActorRef<TestActor> testActor)
    {

    }
}

