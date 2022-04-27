using System;
using System.Linq.Expressions;

using Akka.Actor;

namespace WinTail.Typed;

public class TypedProps
{
    public static TypedProps<TActor> Create<TActor>(Expression<Func<TActor>> factory, SupervisorStrategy supervisorStrategy = null)
        where TActor : Actor<TActor>
        => new(Props.Create(factory, supervisorStrategy));
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
    public static IActorRef<TActor, TMessage> Is<TActor, TMessage>(this IActorRef actor)
        where TActor : Actor<TActor, TMessage>
        => new ActorRefWrapper<TActor, TMessage>(actor);

    public static IActorRef<TActor, TMessage> ActorOf<TActor, TMessage>(this ActorSystem actorSystem, TypedProps<TActor> props, string name = null)
        where TActor : Actor<TActor, TMessage>
        => actorSystem.ActorOf((Props)props, name ?? typeof(TActor).Name).Is<TActor, TMessage>();
}