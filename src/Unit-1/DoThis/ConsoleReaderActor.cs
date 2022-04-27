using System;

using Akka.Actor;

using WinTail.Typed;

namespace WinTail;

public abstract record ConsoleReaderMessage : ActorMessage { internal ConsoleReaderMessage() { } }
/// <summary>
/// Actor responsible for reading FROM the console. 
/// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
/// </summary>
public class ConsoleReaderActor : Actor<ConsoleReaderActor, ConsoleReaderMessage>
{
    public class Messages
    {
        public record Start : ConsoleReaderMessage;

        public record Continue : ConsoleReaderMessage;
    }
    private const string ExitCommand = "exit";

    protected override void OnReceive(ConsoleReaderMessage message)
    {
        if (message is Messages.Start)
            DoPrintInstructions();
        if (message is Messages.Start or Messages.Continue)
            GetAndValidateInput();
    }

    private static void GetAndValidateInput()
    {
        var message = Console.ReadLine() ?? string.Empty;
        if (message.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase) is true)
        {
            Context.System.Terminate();
            return;
        }
        Context.ActorSelection<FileValidationMessage>(
                $"akka://{Constants.ActorSystemName}/user/{FileValidationActor.DefaultName}")
            .Tell(new FileValidationActor.Messages.Validate(message));
    }

    #region Private methods

    private static void DoPrintInstructions()
    {
        Console.WriteLine(string.Join(Environment.NewLine,
            "please provide the URI of a log file on disk", ""
            ));
    }


    #endregion
}
