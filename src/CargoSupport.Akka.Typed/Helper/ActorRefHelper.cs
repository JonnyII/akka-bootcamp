using System.Linq.Expressions;

using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Helper;

public static class ActorRefHelper
{
    /// <summary>
    ///     interprets the ActorRef to be of the specified Actor Type.<br />
    ///     This does not provide runtime TypeChecks!
    ///     if the actor does not have the specified type, no exception will be thrown and the messages will be send
    ///     regardless, without Error!
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="actor"></param>
    /// <returns></returns>
    public static IActorRef<TCommand> HasType<TCommand>(this IActorRef actor)
        where TCommand : FrameworkMessages.Command
    {
        return new ActorRefWrapper<TCommand>(actor);
    }

    public static IEventActorRef<TCommand, TEvent> HasType<TCommand, TEvent>(this IActorRef actor)
        where TCommand : FrameworkMessages.Command
        where TEvent : FrameworkMessages.Event
    {
        return new EventActorRefWrapper<TCommand, TEvent>(actor);
    }

    public static IActorSelection<TCommand> HasType<TCommand>(this ActorSelection selection)
        where TCommand : FrameworkMessages.Command
    {
        return new ActorSelectionWrapper<TCommand>(selection);
    }

    public static IActorSelection<TCommand> ActorSelection<TCommand>(
        this IUntypedActorContext actorSystem, string path)
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorSelection(path).HasType<TCommand>();
    }

    #region ActorOf

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this ActorSystem actorSystem,
        Expression<Func<TActor>> factory, string? name = null)
        where TActor : ActorBase, IActor<TCommand>
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? ActorHelper.GetDefaultName<TActor>())
            .HasType<TCommand>();
    }

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this IUntypedActorContext actorSystem,
        Expression<Func<TActor>> factory, string? name = null)
        where TActor : ActorBase, IActor<TCommand>
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)TypedProps.Create(factory), name ?? ActorHelper.GetDefaultName<TActor>())
            .HasType<TCommand>();
    }

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this ActorSystem actorSystem, string? name = null)
        where TActor : ActorBase, IActor<TCommand>, new()
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? ActorHelper.GetDefaultName<TActor>())
            .HasType<TCommand>();
    }

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this IUntypedActorContext actorSystem,
        string? name = null)
        where TActor : ActorBase, IActor<TCommand>, new()
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? ActorHelper.GetDefaultName<TActor>())
            .HasType<TCommand>();
    }

    public static IEventActorRef<TCommand, TEvent> ActorOf<TActor, TCommand, TEvent>(
        this IUntypedActorContext actorSystem, string? name = null)
        where TActor : ActorBase, IActor<TCommand>, new()
        where TCommand : FrameworkMessages.Command
        where TEvent : FrameworkMessages.Event
    {
        return actorSystem.ActorOf((Props)TypedProps.Create<TActor>(), name ?? ActorHelper.GetDefaultName<TActor>())
            .HasType<TCommand, TEvent>();
    }

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this ActorSystem actorSystem, TypedProps<TActor> props,
        string? name = null)
        where TActor : ActorBase, IActor<TCommand>
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)props, name ?? ActorHelper.GetDefaultName<TActor>()).HasType<TCommand>();
    }

    public static IActorRef<TCommand> ActorOf<TActor, TCommand>(this IUntypedActorContext actorSystem,
        TypedProps<TActor> props, string? name = null)
        where TActor : ActorBase, IActor<TCommand>
        where TCommand : FrameworkMessages.Command
    {
        return actorSystem.ActorOf((Props)props, name ?? ActorHelper.GetDefaultName<TActor>()).HasType<TCommand>();
    }

    #endregion
}