using Akka.Actor;

using WinTail;
using WinTail.Typed;

var myActorSystem = ActorSystem.Create("MainActorSystem");

var consoleWriterActor = myActorSystem.ActorOf<ConsoleWriterActor, ConsoleWriterMessage>();
var tailCoordinationActor = myActorSystem.ActorOf<TailCoordinatorActor, TailCoordinatorMessage>();
var fileValidationActor = myActorSystem.ActorOf<FileValidationActor, FileValidationMessage>(() => new(consoleWriterActor, tailCoordinationActor));
var consoleReaderActor = myActorSystem.ActorOf<ConsoleReaderActor, ConsoleReaderMessage>(() => new(fileValidationActor));

consoleReaderActor.Tell(new ConsoleReaderActor.Messages.Start());

myActorSystem.WhenTerminated.Wait();