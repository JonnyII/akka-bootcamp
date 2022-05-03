
using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Messages;

namespace ChartApp;

public record TestCommand : FrameworkMessages.ActorCommand;

public record TestEvent : FrameworkMessages.ActorEventMessage;

public record SpecificTestEvent : TestEvent;
public class TestEventActor : EventActor<TestCommand, TestEvent>
{
    public TestEventActor()
    {
        this.Receive<TestCommand>(msg =>
        {
            this.PublishEvent(new TestEvent());
        });
    }
}

public record OtherCommand : FrameworkMessages.ActorCommand;

public class EventReceiver : SubscribingActor<OtherCommand>
{
    public EventReceiver()
    {
        IEventActorRef<TestCommand, TestEvent> actor = null!;
        this.GetEventStream<TestEvent, SpecificTestEvent>(actor)
            .Subscribe(data =>
        {

        });

    }

}

internal static class Program
{
    /// <summary>
    /// ActorSystem we'll be using to publish data to charts
    /// and subscribe from performance counters
    /// </summary>
    public static ActorSystem ChartActors = ActorSystem.Create("ChartActors");

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Main());
    }
}
