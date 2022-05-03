using System.Diagnostics;

using Akka.Actor;

using CargoSupport.Akka.Typed;

namespace ChartApp.Actors;

public record PerformanceCounterCommand : FrameworkMessages.ActorCommand;
public record PerformanceCounterEvent : FrameworkMessages.ActorEventMessage;
internal class PerformanceCounterActor : EventActor<PerformanceCounterCommand, PerformanceCounterEvent>
{
    private readonly string _seriesName;
    private readonly Func<PerformanceCounter> _performanceCounterGenerator;
    private PerformanceCounter? _counter;

    private readonly HashSet<IActorRef> _subscriptions = new();
    private readonly ICancelable _cancelPublishing = new Cancelable(Context.System.Scheduler);

    public static class Messages
    {
        public record GatherMetrics : PerformanceCounterCommand;
    }

    public static class Events
    {
        public record MetricUpdate(string Series, float CounterValue) : PerformanceCounterEvent;
    }

    public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
    {
        _seriesName = seriesName;
        _performanceCounterGenerator = performanceCounterGenerator;

        this.Receive<Messages.GatherMetrics>(
            () => PublishEvent(new Events.MetricUpdate(_seriesName, _counter!.NextValue())));
    }

    protected override void PreStart()
    {
        _counter = _performanceCounterGenerator();
        Context.System.Scheduler.ScheduleTellRepeatedly(
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(250),
            Self,
            new Messages.GatherMetrics(),
            Self,
            _cancelPublishing
        );
    }

    protected override void PostStop()
    {
        try
        {
            _cancelPublishing.Cancel(false);
            _counter?.Dispose();
        }
        finally
        {
            base.PostStop();
        }
    }
}
