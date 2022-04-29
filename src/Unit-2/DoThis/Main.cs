using Akka.Util.Internal;

using CargoSupport.Akka.Typed;

using ChartApp.Actors;

namespace ChartApp
{

    public partial class Main : Form
    {
        private IActorRef<ChartingMessage>? _chartActor;
        private readonly AtomicCounter _seriesCounter = new(1);

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization


        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf<ChartingActor, ChartingMessage>(() => new(sysChart), "charting");
            var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
            _chartActor.Tell(new ChartingActor.Messages.InitializeChart(new()
            {
                { series.Name, series }
            }));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor!.Tell(SystemMessages.PoisonPill);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        #endregion

        private void addSeries_Click(object sender, EventArgs e)
        {

        }
    }
}
