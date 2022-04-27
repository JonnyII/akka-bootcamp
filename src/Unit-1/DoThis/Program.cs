using Akka.Actor;

using WinTail;
using WinTail.Typed;
var myActorSystem = ActorSystem.Create(Constants.ActorSystemName);

var consoleWriterActor = myActorSystem.ActorOf<ConsoleWriterActor, ConsoleWriterMessage>();
var tailCoordinationActor = myActorSystem.ActorOf<TailCoordinatorActor, TailCoordinatorMessage>();
var fileValidationActor = myActorSystem.ActorOf<FileValidationActor, FileValidationMessage>(() => new(consoleWriterActor));
var consoleReaderActor = myActorSystem.ActorOf<ConsoleReaderActor, ConsoleReaderMessage>();

consoleReaderActor.Tell(new ConsoleReaderActor.Messages.Start());

myActorSystem.WhenTerminated.Wait();
public static class Constants
{
    public const string ActorSystemName = "MainActorSystem";
}