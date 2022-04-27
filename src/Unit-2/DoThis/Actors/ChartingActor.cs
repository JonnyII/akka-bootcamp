
using System.Windows.Forms.DataVisualization.Charting;

using CargoSupport.Akka.Typed;

namespace ChartApp.Actors;
public record ChartingMessage : ActorMessage { }
public class ChartingActor : Actor<ChartingActor, ChartingMessage>
{
    public class Messages
    {
        public record InitializeChart(Dictionary<string, Series>? InitialSeries) : ChartingMessage;
    }

    private readonly Chart _chart;
    private Dictionary<string, Series> _seriesIndex;

    public ChartingActor(Chart chart) : this(chart, new())
    {
    }

    public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
    {
        _chart = chart;
        _seriesIndex = seriesIndex;
    }

    protected override void OnReceive(ChartingMessage message)
    {
        switch (message)
        {
            case Messages.InitializeChart initChart:
                HandleInitialize(initChart);
                break;
        }
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

    #endregion
}
