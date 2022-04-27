using Akka.Actor;

using WinTail;
using WinTail.Typed;

var myActorSystem = ActorSystem.Create("MainActorSystem");

var consoleWriterProps = TypedProps.Create(() => new ConsoleWriterActor());
var consoleWriterActor = myActorSystem.ActorOf<ConsoleWriterActor, ConsoleWriterMessage>(consoleWriterProps);

var validationActorProps = TypedProps.Create(() => new ValidationActor(consoleWriterActor));
var validationActor = myActorSystem.ActorOf<ValidationActor, ValidationMessage>(validationActorProps);

var consoleReaderProps = TypedProps.Create(() => new ConsoleReaderActor(validationActor));
var consoleReaderActor = myActorSystem.ActorOf<ConsoleReaderActor, ConsoleReaderMessage>(consoleReaderProps);

consoleReaderActor.Tell(new ConsoleReaderActor.Messages.Start());

myActorSystem.WhenTerminated.Wait();