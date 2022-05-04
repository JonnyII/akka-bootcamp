using Akka.Actor;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.ActorRef;

public interface ICanTell<TCommand>
    where TCommand : FrameworkMessages.Command
{
    internal ICanTell Native { get; }
    void Tell(TCommand command, IActorRef<TCommand>? sender = null);
}