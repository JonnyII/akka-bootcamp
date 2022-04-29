using System.Linq.Expressions;

using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public class TypedProps
{
    public static TypedProps<TActor> Create<TActor>(Expression<Func<TActor>> factory, SupervisorStrategy? supervisorStrategy = null)
        where TActor : ActorBase
        => new(Props.Create(factory, supervisorStrategy));
    public static TypedProps<TActor> Create<TActor>(SupervisorStrategy? supervisorStrategy = null)
        where TActor : ActorBase, new()
        => new(Props.Create(() => new TActor(), supervisorStrategy));
}

public class TypedProps<TActor>
    where TActor : ActorBase
{
    private readonly Props _source;

    internal TypedProps(Props source)
    {
        _source = source;
    }

    public static explicit operator Props(TypedProps<TActor> props)
        => props._source;
}