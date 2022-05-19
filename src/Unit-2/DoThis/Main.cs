using System;
using System.Windows.Forms;

using Akka.Actor;
using Akka.Util.Internal;

using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private IActorRef _chartActor;
        private readonly AtomicCounter _seriesCounter = new(1);

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization


        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
            _chartActor.Tell(new ChartingActor.InitializeChart(new()
            {
                { series.Name, series }
            }));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            _chartActor.Tell(new ChartingActor.AddSeries(
                ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement())
                ));
        }
    }
}
