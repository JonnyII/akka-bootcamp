
using WinTail.Typed;

namespace WinTail;

public abstract record ValidationMessage
{
    internal ValidationMessage() { }
}
public class ValidationActor : Actor<ValidationActor, ValidationMessage>
{
    public class Messages
    {
        public record Validate(string Text) : ValidationMessage;
    }

    public class ConsoleWriterExtensionMethods
    {
        public record InputError(string Reason) : ConsoleWriterActor.Messages.Error(Reason);
        public record InputSuccess() : ConsoleWriterActor.Messages.Success("Thank you! Text was valid.");

        public record NullInputError() : InputError("no input received");
        public record ValidationError(string Reason) : InputError(Reason);

    }
    private readonly IActorRef<ConsoleWriterActor, ConsoleWriterMessage> _consoleWriterActor;

    public ValidationActor(IActorRef<ConsoleWriterActor, ConsoleWriterMessage> consoleWriterActor)
    {
        _consoleWriterActor = consoleWriterActor;
    }

    protected override void OnReceive(ValidationMessage rawMessage)
    {
        if (rawMessage is Messages.Validate validationMessage)
            HandleValidationRequest(validationMessage.Text);

        Sender.Is<ConsoleReaderActor, ConsoleReaderMessage>().Tell(new ConsoleReaderActor.Messages.Continue());
    }

    private void HandleValidationRequest(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            _consoleWriterActor.Tell(new ConsoleWriterExtensionMethods.NullInputError());
            return;
        }

        if (IsFaulty(message))
        {
            _consoleWriterActor.Tell(new ConsoleWriterExtensionMethods.ValidationError("Invalid: input had odd number of characters."));
            return;
        }
        _consoleWriterActor.Tell(new ConsoleWriterExtensionMethods.InputSuccess());
    }
    private static bool IsFaulty(string message)
        => message.Length % 2 != 0;
}
