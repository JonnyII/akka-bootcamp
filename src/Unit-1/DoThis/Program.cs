using Akka.Actor;

using WinTail;
using WinTail.Typed;


interface ITestActorCommand { }
class TestActor : Actor<TestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message) { }
}
class DependentTestActor : Actor<DependentTestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message) { }
    public DependentTestActor(IActorRef<TestActor> testActor)
    {
    }
}



var myActorSystem = ActorSystem.Create("MainActorSystem");

TypedProps<TestActor> test1 = TypedProps.Create<TestActor>();
IActorRef<TestActor> testActor1 = myActorSystem.ActorOf(test1);

TypedProps<TestActor> test2 = TypedProps.Create(() => new TestActor());
IActorRef<TestActor> testActor2 = myActorSystem.ActorOf(test2);

TypedProps<DependentTestActor> test3 = TypedProps.Create<DependentTestActor>();
IActorRef<DependentTestActor> testActor3 = myActorSystem.ActorOf(test3);

TypedProps<DependentTestActor> test4 = TypedProps.Create(() => new DependentTestActor(testActor1));
IActorRef<DependentTestActor> testActor4 = myActorSystem.ActorOf(test4);

var consoleWriterProps = Props.Create(() => new ConsoleWriterActor());
var consoleWriterActor = myActorSystem.ActorOf(consoleWriterProps, nameof(ConsoleWriterActor));

var validationActorProps = Props.Create(() => new ValidationActor(consoleWriterActor));
var validationActor = myActorSystem.ActorOf(validationActorProps, nameof(ValidationActor));

var consoleReaderProps = Props.Create(() => new ConsoleReaderActor(validationActor));
var consoleReaderActor = myActorSystem.ActorOf(consoleReaderProps, nameof(ConsoleReaderActor));

consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

myActorSystem.WhenTerminated.Wait();
