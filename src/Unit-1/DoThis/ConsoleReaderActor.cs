using System;

using Akka.Actor;

using WinTail.Typed;

namespace WinTail;

public abstract record ConsoleReaderMessage
{
    internal ConsoleReaderMessage() { }
}
/// <summary>
/// Actor responsible for reading FROM the console. 
/// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
/// </summary>
class ConsoleReaderActor : Actor<ConsoleReaderActor, ConsoleReaderMessage>
{
    public class Messages
    {
        public record Start : ConsoleReaderMessage;

        public record Continue : ConsoleReaderMessage;
    }
    private readonly IActorRef<ValidationActor, ValidationMessage> _validationActor;

    //todo/check: use enum instead of constant strings later on
    public const string ExitCommand = "exit";
    public const string StartCommand = "start";

    public ConsoleReaderActor(IActorRef<ValidationActor, ValidationMessage> validationActor)
    {
        _validationActor = validationActor;
    }

    protected override void OnReceive(ConsoleReaderMessage message)
    {
        if (message is Messages.Start)
            DoPrintInstructions();
        if (message is Messages.Start or Messages.Continue)
            GetAndValidateInput();
    }

    private void GetAndValidateInput()
    {
        var message = Console.ReadLine();
        if (message?.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase) is true)
        {
            Context.System.Terminate();
            return;
        }
        _validationActor.Tell(new ValidationActor.Messages.Validate(message));
    }

    #region Private methods

    private static void DoPrintInstructions()
    {
        Console.WriteLine(string.Join(Environment.NewLine,
            "Write whatever you want into the console!",
            "Some entries will pass validation, and some won't...", "",
            "Type 'exit' to quit this application at any time."
            ));
    }


    #endregion
}
