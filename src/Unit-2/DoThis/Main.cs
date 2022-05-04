using Akka.Actor;
using Akka.Util.Internal;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Helper;
using CargoSupport.Akka.Typed.Messages;

using ChartApp.Actors;

namespace ChartApp;

public enum CounterType
{
    Cpu,
    Memory,
    Disk
}

public record TutorialSubscriptionMessage;

public record SubscribeCounter(CounterType Counter, IActorRef Subscriber) : TutorialSubscriptionMessage;

public record UnsubscribeCounter(CounterType Counter, IActorRef Subscriber) : TutorialSubscriptionMessage;

public partial class Main : Form
{
    private readonly AtomicCounter _seriesCounter = new(1);
    private IActorRef<Charting.Commands>? _chartActor;

    public Main()
    {
        InitializeComponent();
    }

    private void Cpu_Click(object sender, EventArgs e)
    {
    }

    private void Memory_Click(object sender, EventArgs e)
    {
    }

    private void Disk_Click(object sender, EventArgs e)
    {
    }

    #region Initialization

    private void Main_Load(object sender, EventArgs e)
    {
        _chartActor = Program.ChartActors.ActorOf<Charting.Actor, Charting.Commands>(() => new(sysChart), "charting");
        var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
        _chartActor.Tell(new Charting.Commands.InitializeChart(new() { { series.Name, series } }));
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        //shut down the charting actor
        _chartActor!.Tell(SystemMessages.PoisonPill);

        //shut down the ActorSystem
        Program.ChartActors.Terminate();
    }

    #endregion
}
