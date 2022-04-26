using System;

using Akka.Actor;

namespace WinTail;

/// <summary>
/// Actor responsible for serializing message writes to the console.
/// (write one message at a time, champ :)
/// </summary>
class ConsoleWriterActor : UntypedActor
{
    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case Messages.InputError inputErrorMessage:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(inputErrorMessage.Reason);
                break;
            case Messages.InputSuccess inputSuccessMessage:
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
