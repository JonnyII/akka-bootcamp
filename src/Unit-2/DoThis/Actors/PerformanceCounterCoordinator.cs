using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

using DevExpress.XtraCharts;

using SysPerformanceCounter = System.Diagnostics.PerformanceCounter;


namespace ChartApp.Actors;

public class PerformanceCounterCoordinator
{
    public record Commands : FrameworkMessages.Command
    {
        public record Watch(CounterType Counter) : Commands;

        public record Unwatch(CounterType Counter) : Commands;
    }

    public class Actor : ReceiveActor<Commands>
    {
        private static readonly Dictionary<CounterType, Func<SysPerformanceCounter>>
            CounterGenerators = new()
            {
                { CounterType.Cpu, () => new("Processor", "% processor Time", "_Total", true) },
                { CounterType.Memory, () => new("Memory", "% Committed Bytes In Use", true) },
                { CounterType.Disk, () => new("LogicalDisk", "% Disk Tune", true) }
            };

        private static readonly Dictionary<CounterType, Func<Series>>
            CounterSeries = new()
            {
                { CounterType.Cpu, () => new(CounterType.Cpu.ToString(), ViewType.Line) },
                { CounterType.Memory, () => new(CounterType.Memory.ToString(), ViewType.Line) },
                { CounterType.Disk, () => new(CounterType.Disk.ToString(), ViewType.SplineArea) }
            };

        private readonly IActorRef<Charting.Commands> _chartingActor;

        private readonly Dictionary<CounterType, PerformanceCounter.IActorRef>
            _counterActors;

        public Actor(IActorRef<Charting.Commands> chartingActor)
            : this(chartingActor, new())
        {
        }

        public Actor(
            IActorRef<Charting.Commands> chartingActor,
            Dictionary<CounterType, PerformanceCounter.IActorRef> counterActors)
        {
            _chartingActor = chartingActor;
            _counterActors = counterActors;
            Receive<Commands.Watch>(watch =>
            {
                var counterActor = _counterActors.GetOrAdd(
                    watch.Counter,
                    () => Context.ActorOfPerformanceCounter(
                        watch.Counter.ToString(),
                        CounterGenerators[watch.Counter]));
                _chartingActor.Tell(
                    new Charting.Commands.AddSeries(CounterSeries[watch.Counter]()));
                counterActor.Tell();
            });
        }
    }
}
