using System;

using Akka.Actor;

namespace WinTail;
/// <summary>
/// Actor responsible for reading FROM the console. 
/// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
/// </summary>
class ConsoleReaderActor : UntypedActor
{
    private readonly IActorRef _validationActor;

    //todo/check: use enum instead of constant strings later on
    public const string ExitCommand = "exit";
    public const string StartCommand = "start";

    public ConsoleReaderActor(IActorRef validationActor)
    {
        _validationActor = validationActor;
    }

    protected override void OnReceive(object message)
    {
        if (message is StartCommand)
            DoPrintInstructions();
        if (message is StartCommand or Messages.ContinueProcessing)
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
        _validationActor.Tell(message);
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
