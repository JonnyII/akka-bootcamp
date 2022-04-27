using Akka.Actor;
using Akka.Util;

namespace WinTail.Typed;

public interface IActorRef<TActor, in TActorMessageBase>
    where TActor : Actor<TActor, TActorMessageBase>
{
    void Tell(TActorMessageBase message);
    ISurrogate ToSurrogate(ActorSystem system);
    ActorPath Path { get; }
}
public class ActorRefWrapper<TActor, TActorMessageBase> : IActorRef<TActor, TActorMessageBase>
    where TActor : Actor<TActor, TActorMessageBase>
{
    private readonly IActorRef _sourceRef;

    internal ActorRefWrapper(IActorRef sourceRef)
    {
        _sourceRef = sourceRef;
    }

    public void Tell(TActorMessageBase message)
        => _sourceRef.Tell(message, ActorCell.GetCurrentSelfOrNoSender());// 2nd parameter extracted from ActorRefImplicitSenderExtensions

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