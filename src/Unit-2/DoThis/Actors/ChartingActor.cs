using Akka.Actor;

using CargoSupport.Akka.Typed;

using DevExpress.XtraCharts;

namespace ChartApp.Actors;
public record ChartingMessage : ActorMessage { }
public class ChartingActor : ReceiveActor<ChartingActor, ChartingMessage>
{
    public class Messages
    {
        public record InitializeChart(Dictionary<string, Series>? InitialSeries) : ChartingMessage;

        public record AddSeries : ChartingMessage
        {
            public AddSeries(Series series)
            {
                if (string.IsNullOrWhiteSpace(series.Name))
                    throw new InvalidMessageException("The series name must not be empty.");
                this.Series = series;
            }

            public Series Series { get; init; }

            public void Deconstruct(out Series series)
            {
                series = this.Series;
            }
        }
    }

    private readonly ChartControl _chart;
    private Dictionary<string, Series> _seriesIndex;

    public ChartingActor(ChartControl chart) : this(chart, new())
    {
    }

    public ChartingActor(ChartControl chart, Dictionary<string, Series> seriesIndex)
    {
        _chart = chart;
        _seriesIndex = seriesIndex;

        Receive<Messages.InitializeChart>(HandleInitialize);
        Receive<Messages.AddSeries>(HandleAddSeries);
    }

    #region Individual Message Type Handlers

    private void HandleInitialize(Messages.InitializeChart ic)
    {
        if (ic.InitialSeries is not null)
            //swap the two series out
            _seriesIndex = ic.InitialSeries;

        //delete any existing series
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

    private void HandleAddSeries(Messages.AddSeries series)
    {

    }
    #endregion
}
