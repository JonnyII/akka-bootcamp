
using Akka.Actor;

namespace WinTail;
public class ValidationActor : UntypedActor
{
    private readonly IActorRef _consoleWriterActor;

    public ValidationActor(IActorRef consoleWriterActor)
    {
        _consoleWriterActor = consoleWriterActor;
    }

    protected override void OnReceive(object rawMessage)
    {
        if (rawMessage is not string message)
            _consoleWriterActor.Tell($"message was not of type string but {rawMessage.GetType().Name} instead.");
        else
            HandleStringMessage(message);

        Sender.Tell(new Messages.ContinueProcessing());
    }

    private void HandleStringMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            _consoleWriterActor.Tell(new Messages.NullInputError("no input received"));
            return;
        }

        if (IsFaulty(message))
        {
            _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
            return;
        }
        _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
    }
    private static bool IsFaulty(string message)
        => message.Length % 2 != 0;
}
