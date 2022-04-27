using System;
using CargoSupport.Akka.Typed;

namespace WinTail;


public abstract record ConsoleWriterMessage : ActorMessage { internal ConsoleWriterMessage() { } }
/// <summary>
/// Actor responsible for serializing message writes to the console.
/// (write one message at a time, champ :)
/// </summary>
public class ConsoleWriterActor : Actor<ConsoleWriterActor, ConsoleWriterMessage>
{
    public class Messages
    {
        public record Error(string Reason) : ConsoleWriterMessage;
        public record Success(string Reason) : ConsoleWriterMessage;
    }
    protected override void OnReceive(ConsoleWriterMessage message)
    {
        switch (message)
        {
            case Messages.Error inputErrorMessage:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(inputErrorMessage.Reason);
                break;
            case Messages.Success inputSuccessMessage:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(inputSuccessMessage.Reason);
                break;
            default:
                Console.WriteLine(message);
                break;
        }
        Console.ResetColor();
    }
}
