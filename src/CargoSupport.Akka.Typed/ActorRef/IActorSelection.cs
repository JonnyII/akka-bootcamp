using Akka.Actor;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface IActorSelection<TCommandBase> : ICanTell<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    IActorRef<TCommandBase> Anchor { get; }
    SelectionPathElement[] Path { get; }
    string PathString { get; }
    Task<IActorRef<TCommandBase>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null);
}

public class ActorSelectionWrapper<TCommandBase> : IActorSelection<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    private readonly ActorSelection _actorSelection;

    public ActorSelectionWrapper(ActorSelection actorSelection)
    {
        _actorSelection = actorSelection;
    }

    public IActorRef<TCommandBase> Anchor => _actorSelection.Anchor.HasType<TCommandBase>();
    public SelectionPathElement[] Path => _actorSelection.Path;
    public string PathString => _actorSelection.PathString;

    public void Tell(TCommandBase command, IActorRef<TCommandBase>? sender = null)
    {
        _actorSelection.Tell(command, sender?.Native);
    }

    ICanTell ICanTell<TCommandBase>.Native => _actorSelection;

    public Task<IActorRef<TCommandBase>> ResolveOne(TimeSpan timeout, CancellationToken? ct = null)
    {
        return _actorSelection
            .ResolveOne(timeout, ct ?? CancellationToken.None)
            .ContinueWith(actor => actor.Result.HasType<TCommandBase>(), ct ?? CancellationToken.None);
    }
}