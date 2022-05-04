using Akka.Actor;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface IPathActor
{
    ActorPath Path { get; }
}