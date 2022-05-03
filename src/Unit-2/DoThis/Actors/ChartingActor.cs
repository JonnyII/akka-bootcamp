using Akka.Actor;

using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Messages;

using DevExpress.XtraCharts;

namespace ChartApp.Actors;
public static class Charting
{
    public abstract record Commands : FrameworkMessages.Command
    {

        public record InitializeChart(Dictionary<string, Series>? InitialSeries) : Commands;

        public record AddSeries : Commands
        {
            public AddSeries(Series series)
            {
                if (string.IsNullOrWhiteSpace(series.Name))
                    throw new InvalidMessageException("The seriesMessage name must not be empty.");
                this.Series = series;
            }

            public Series Series { get; init; }

            public void Deconstruct(out Series series)
            {
                series = this.Series;
            }
        }
    }



    public class Actor : ReceiveActor<Commands>
    {
        private readonly ChartControl _chart;
        private Dictionary<string, Series> _seriesIndex;

        public Actor(ChartControl chart) : this(chart, new())
        {
        }

        public Actor(ChartControl chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<Commands.InitializeChart>(HandleInitialize);
            Receive<Commands.AddSeries>(HandleAddSeries);
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(Commands.InitializeChart ic)
        {
            if (ic.InitialSeries is not null)
                //swap the two seriesMessage out
                _seriesIndex = ic.InitialSeries;

            //delete any existing seriesMessage
            _chart.Series.Clear();

            //attempt to render the initial chart
            if (!_seriesIndex.Any())
                return;

            foreach (var (key, value) in _seriesIndex)
            {
                //force both the chart and the internal index to use the same names
                value.Name = key;
                _chart.Series.Add(value);
            }
        }

        private void HandleAddSeries(Commands.AddSeries seriesMessage)
        {
            if (_seriesIndex.ContainsKey(seriesMessage.Series.Name))
                return;
            _seriesIndex.Add(seriesMessage.Series.Name, seriesMessage.Series);
            _chart.Series.Add(seriesMessage.Series);
        }

        #endregion
    }
}