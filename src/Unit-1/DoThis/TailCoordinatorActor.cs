
using Akka.Actor;

namespace WinTail;

public class TailCoordinatorActor : UntypedActor
{
    #region message types
    public record StartTail(string FilePath, IActorRef ReporterActor);
    public record StopTail(string FilePath);
    #endregion
    protected override void OnReceive(object message)
    {
        if (message is StartTail startTailRequest)
        {
        }
    }
}
