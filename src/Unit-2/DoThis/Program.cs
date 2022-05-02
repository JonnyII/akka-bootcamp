
using Akka.Actor;

using CargoSupport.Akka.Typed;

namespace ChartApp;

public record TestMessage : ActorMessage;

public record TestEvent : ActorEventMessage;

public record SpecificTestEvent : TestEvent;
public class TestEventActor : EventActor<TestMessage, TestEvent>
{
    public TestEventActor()
    {
        this.Receive<TestMessage>(msg =>
        {
            this.PublishEvent(new TestEvent());
        });
    }
}

public record OtherMessage : ActorMessage;

public class EventReceiver : SubscribingActor<OtherMessage>
{
    public EventReceiver()
    {
        IEventActorRef<TestMessage, TestEvent> actor = null!;
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
