using System.Reactive.Disposables;

using Akka.Actor;
using Akka.Util;

using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface ICanTell<TCommand>
    where TCommand : FrameworkMessages.Command
{
    void Tell(TCommand command, IActorRef<TCommand>? sender = null);
    internal ICanTell Native { get; }
}

public interface IActorRef<TCommandBase> : ICanTell<TCommandBase>, IPathActor
    where TCommandBase : FrameworkMessages.Command
{
    ISurrogate ToSurrogate(ActorSystem system);
    ICanTell ICanTell<TCommandBase>.Native => Native;
    internal new IActorRef Native { get; }

    void Tell(SystemMessages.SystemCommand command, IActorRef<TCommandBase>? sender = null);
}

public interface IPathActor
{
    ActorPath Path { get; }
}
internal class ActorRefWrapper<TCommandBase> : IActorRef<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    protected readonly IActorRef Source;
    IActorRef IActorRef<TCommandBase>.Native => Source;


    internal ActorRefWrapper(IActorRef source)
    {
        Source = source;
    }

    public void Tell(TCommandBase command, IActorRef<TCommandBase>? sender = null)
        => Source.Tell(command, sender?.Native ?? ActorCell.GetCurrentSelfOrNoSender());// 2nd parameter extracted from ActorRefImplicitSenderExtensions


    public void Tell(SystemMessages.SystemCommand command, IActorRef<TCommandBase>? sender = null)
        => Source.Tell(command.GetNative());

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


public interface IActorSelection<TCommandBase> : ICanTell<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    IActorRef<TCommandBase> Anchor { get; }
    SelectionPathElement[] Path { get; }
    string PathString { get; }
    Task<IActorRef<TCommandBase>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null);
}

public class ActorSelectionWrapper<TComamndBase> : IActorSelection<TComamndBase>
    where TComamndBase : FrameworkMessages.Command
{
    private readonly ActorSelection _actorSelection;

    public ActorSelectionWrapper(ActorSelection actorSelection)
    {
        _actorSelection = actorSelection;
    }

    public IActorRef<TComamndBase> Anchor => _actorSelection.Anchor.Receives<TComamndBase>();
    public SelectionPathElement[] Path => _actorSelection.Path;
    public string PathString => _actorSelection.PathString;
    public void Tell(TComamndBase command, IActorRef<TComamndBase>? sender = null)
        => _actorSelection.Tell(command, sender?.Native);

    ICanTell ICanTell<TComamndBase>.Native => _actorSelection;

    public Task<IActorRef<TComamndBase>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null)
        => _actorSelection
            .ResolveOne(timeout, ct ?? CancellationToken.None)
            .ContinueWith(actor => actor.Result.Receives<TComamndBase>(), ct ?? CancellationToken.None);
}

public interface IEventActorRef<in TEventBase> : IPathActor
    where TEventBase : FrameworkMessages.Event
{
    /// <summary>
    /// subscribes the current actor to the event stream of this actorRef
    /// sends a <see cref="FrameworkMessages.Subscribe"/> to the referenced actor
    /// sends a <see cref="FrameworkMessages.Unsubscribe"/> once the result-disposable has been disposed of
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

internal class EventActorRefWrapper<TCommandBase, TEventBase> : ActorRefWrapper<TCommandBase>, IEventActorRef<TCommandBase, TEventBase>
    where TCommandBase : FrameworkMessages.Command
    where TEventBase : FrameworkMessages.Event
{
    internal EventActorRefWrapper(IActorRef source) : base(source)
    {
    }

    /// <summary>
    /// dispose of the disposable to unsubscribe
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public IDisposable ListenTo<TEvent>() where TEvent : TEventBase
    {
        Source.Tell(new FrameworkMessages.Subscribe<TEvent>());
        return Disposable.Create(() => Source.Tell(new FrameworkMessages.Unsubscribe<TEvent>()));
    }
}
