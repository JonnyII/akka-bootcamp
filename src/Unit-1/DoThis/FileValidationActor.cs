
using System;
using System.IO;

using WinTail.Typed;

using IConsoleWriterActorRef = WinTail.Typed.IActorRef<WinTail.ConsoleWriterMessage>;
namespace WinTail;

public abstract record FileValidationMessage : ActorMessage { internal FileValidationMessage() { } }
public class FileValidationActor : Actor<FileValidationActor, FileValidationMessage>
{
    public class Messages
    {
        public record Validate(string Content) : FileValidationMessage;
    }

    public class ConsoleWriterExtensions
    {
        public record NullInputError(string Reason) : ConsoleWriterActor.Messages.Error(Reason);
        public record ValidationError(string Reason) : ConsoleWriterActor.Messages.Error(Reason);
        public record InputSuccess(string Reason) : ConsoleWriterActor.Messages.Success(Reason);
    }
    private readonly IConsoleWriterActorRef _consoleWriteActor;

    public FileValidationActor(IConsoleWriterActorRef consoleWriteActor)
    {
        _consoleWriteActor = consoleWriteActor;
    }

    protected override void OnReceive(FileValidationMessage message)
    {
        switch (message)
        {
            case Messages.Validate(var content) when string.IsNullOrEmpty(content):

                _consoleWriteActor.Tell(new ConsoleWriterExtensions.NullInputError(
                    "Input was blank. Please try again." + Environment.NewLine));
                Sender.Receives<ConsoleReaderMessage>().Tell(new ConsoleReaderActor.Messages.Continue());
                return;

            case Messages.Validate(var content) when File.Exists(content) is false:

                _consoleWriteActor.Tell(new ConsoleWriterExtensions.ValidationError($"{content} could not be found"));
                // ask for the next filepath
                Sender.Receives<ConsoleReaderMessage>().Tell(new ConsoleReaderActor.Messages.Continue());
                return;

            case Messages.Validate(var content):

                _consoleWriteActor.Tell(new ConsoleWriterExtensions.InputSuccess($"Started processing for {content}"));
                Context.ActorSelection<TailCoordinatorMessage>(
                    $"akka://{Constants.ActorSystemName}/user/{TailCoordinatorActor.DefaultName}")
                    .Tell(new TailCoordinatorActor.Messages.Start(content, _consoleWriteActor));
                return;
        }
    }
}
