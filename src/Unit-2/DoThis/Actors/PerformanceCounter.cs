using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

using SysPerformanceCounter = System.Diagnostics.PerformanceCounter;

namespace ChartApp.Actors;

public static class PerformanceCounter
{
    internal class Actor : EventActor<Commands, Events>
    {
        private readonly ICancelable _cancelPublishing = new Cancelable(Context.System.Scheduler);
        private readonly Func<SysPerformanceCounter> _performanceCounterGenerator;

        private readonly HashSet<IActorRef> _subscriptions = new();
        private SysPerformanceCounter? _counter;


        public Actor(string seriesName, Func<SysPerformanceCounter> performanceCounterGenerator)
        {
            _performanceCounterGenerator = performanceCounterGenerator;
            Receive<Commands.GatherMetrics>(
                () => PublishEvent(new Events.MetricUpdate(seriesName, _counter!.NextValue())));
        }

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(250),
                Self,
                new Commands.GatherMetrics(),
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

    #region Communication

    public static IActorRef ActorOfPerformanceCounter(
        this IActorRefFactory factory,
        string seriesName,
        Func<SysPerformanceCounter> performanceCounterGenerator,
        string? name = null)
    {
        return new Wrapper(
            factory.ActorOf(
                Props.Create(() => new Actor(seriesName, performanceCounterGenerator)),
                name)
        );
    }

    public interface IActorRef : IEventActorRef<Commands, Events>
    {
    }

    private class Wrapper : EventActorRefWrapper<Commands, Events>, IActorRef
    {
        protected internal Wrapper(Akka.Actor.IActorRef native) : base(native)
        {
        }
    }

    public abstract record Commands : FrameworkMessages.Command
    {
        public record GatherMetrics : Commands;
    }

    public abstract record Events : FrameworkMessages.Event
    {
        public record MetricUpdate(string Series, float CounterValue) : Events;
    }

    #endregion
}
