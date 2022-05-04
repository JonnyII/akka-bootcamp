using Akka.Actor;
using Akka.Util;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface IActorRef<TCommandBase> : ICanTell<TCommandBase>, IPathActor, ITypedActorRef
    where TCommandBase : FrameworkMessages.Command
{
    internal new IActorRef Native { get; }
    ICanTell ICanTell<TCommandBase>.Native => Native;
    IActorRef ITypedActorRef.Native => Native;
    ISurrogate ToSurrogate(ActorSystem system);

    void Tell(SystemMessages.SystemCommand command, IActorRef<TCommandBase>? sender = null);
}

public class ActorRefWrapper<TCommandBase> : IActorRef<TCommandBase>
    where TCommandBase : FrameworkMessages.Command
{
    protected readonly IActorRef Native;


    internal ActorRefWrapper(IActorRef native)
    {
        Native = native;
    }

    IActorRef IActorRef<TCommandBase>.Native => Native;

    public void Tell(TCommandBase command, IActorRef<TCommandBase>? sender = null)
    {
        Native.Tell(command, sender?.Native ?? ActorCell.GetCurrentSelfOrNoSender());
        // 2nd parameter extracted from ActorRefImplicitSenderExtensions
    }


    public void Tell(SystemMessages.SystemCommand command, IActorRef<TCommandBase>? sender = null)
    {
        Native.Tell(command.GetNative());
    }

    public ISurrogate ToSurrogate(ActorSystem system)
    {
        return Native.ToSurrogate(system);
    }

    public ActorPath Path
        => Native.Path;

    public bool Equals(IActorRef? other)
    {
        return Native.Equals(other);
    }

    public int CompareTo(IActorRef? other)
    {
        return Native.CompareTo(other);
    }

    public int CompareTo(object? obj)
    {
        return Native.CompareTo(obj);
    }
}