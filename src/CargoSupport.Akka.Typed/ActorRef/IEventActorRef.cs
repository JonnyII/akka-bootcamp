using System.Reactive.Disposables;

using Akka.Actor;

using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface IEventActorRef<in TEventBase> : IPathActor, ITypedActorRef
    where TEventBase : FrameworkMessages.Event
{
    internal new IActorRef Native { get; }
    IActorRef ITypedActorRef.Native => Native;

    /// <summary>
    ///     subscribes the current actor to the event stream of this actorRef
    ///     sends a <see cref="FrameworkMessages.Subscribe" /> to the referenced actor
    ///     sends a <see cref="FrameworkMessages.Unsubscribe" /> once the result-disposable has been disposed of
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IDisposable ListenTo<TEvent>()
        where TEvent : TEventBase;
}

public interface IEventActorRef<TCommandBase, in TEventBase> : IActorRef<TCommandBase>, IEventActorRef<TEventBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
}

public class EventActorRefWrapper<TCommandBase, TEventBase> : ActorRefWrapper<TCommandBase>,
    IEventActorRef<TCommandBase, TEventBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    protected internal EventActorRefWrapper(IActorRef native) : base(native)
    {
    }

    /// <summary>
    ///     dispose of the disposable to unsubscribe
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IDisposable ListenTo<TEvent>() where TEvent : TEventBase
    {
        Native.Tell(new FrameworkMessages.Subscribe<TEvent>());
        return Disposable.Create(() => Native.Tell(new FrameworkMessages.Unsubscribe<TEvent>()));
    }

    IActorRef IEventActorRef<TEventBase>.Native => Native;

    IActorRef ITypedActorRef.Native => Native;
}

public class EventActorRefWrapper<TEventBase> : IEventActorRef<TEventBase>
    where TEventBase : FrameworkMessages.Event
{
    public EventActorRefWrapper(IActorRef native)
    {
        Native = native;
    }

    internal IActorRef Native { get; }
    public ActorPath Path => Native.Path;

    public IDisposable ListenTo<TEvent>() where TEvent : TEventBase
    {
        Native.Tell(new FrameworkMessages.Subscribe<TEvent>());
        return Disposable.Create(() => Native.Tell(new FrameworkMessages.Unsubscribe<TEvent>()));
    }

    IActorRef IEventActorRef<TEventBase>.Native => Native;
    IActorRef ITypedActorRef.Native => Native;
}