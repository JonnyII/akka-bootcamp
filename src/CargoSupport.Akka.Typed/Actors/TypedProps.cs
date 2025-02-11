﻿using System.Linq.Expressions;
using Akka.Actor;

namespace CargoSupport.Akka.Typed.Actors;

public class TypedProps
{
    public static TypedProps<TActor> Create<TActor>(Expression<Func<TActor>> factory,
        SupervisorStrategy? supervisorStrategy = null)
        where TActor : ActorBase
    {
        return new(Props.Create(factory, supervisorStrategy));
    }

    public static TypedProps<TActor> Create<TActor>(SupervisorStrategy? supervisorStrategy = null)
        where TActor : ActorBase, new()
    {
        return new(Props.Create(() => new TActor(), supervisorStrategy));
    }
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
    {
        return props._source;
    }
}