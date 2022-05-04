using Akka.Actor;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface ITypedActorRef
{
    internal IActorRef Native { get; }
}