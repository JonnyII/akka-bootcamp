
using Akka.Actor;

using WinTail.Typed;

namespace WinTail;

public record TailCoordinatorMessage
{
    internal TailCoordinatorMessage() { }
}
public class TailCoordinatorActor : Actor<TailCoordinatorActor, TailCoordinatorMessage>
{
    public class Messages
    {
        public record StartTail(string FilePath, IActorRef ReporterActor) : TailCoordinatorMessage;
        public record StopTail(string FilePath) : TailCoordinatorMessage;
    }
    protected override void OnReceive(TailCoordinatorMessage message)
    {
        if (message is Messages.StartTail startTailRequest)
        {
        }
    }
}
