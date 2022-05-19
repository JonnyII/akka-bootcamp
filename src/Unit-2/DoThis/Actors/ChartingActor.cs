using System.Collections.Generic;
using System.Linq;

using Akka.Actor;

using DevExpress.XtraCharts;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        public ChartControl Chart { get; }

        #region Messages

        public record InitializeChart(Dictionary<string, Series> InitialSeries);

        public record AddSeries(Series Series);

        #endregion

        private readonly ChartControl _chart;
        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(ChartControl chart) : this(chart, new())
        {
            Chart = chart;
        }

        public ChartingActor(ChartControl chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<InitializeChart>(HandleInitialize);
            Receive<AddSeries>(HandleAddSeries);
        }

        private void HandleAddSeries(AddSeries series)
        {
            if (
                    string.IsNullOrEmpty(series.Series.Name)
                    || _seriesIndex.ContainsKey(series.Series.Name))
                return;

            _seriesIndex.Add(series.Series.Name, series.Series);
            _chart.Series.Add(series.Series);
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            _chart.Series?.Clear();

            //attempt to render the initial chart
            if (!_seriesIndex.Any())
                return;

            foreach (var (name, series) in _seriesIndex)
            {
                //force both the chart and the internal index to use the same names
                series.Name = name;
                _chart.Series?.Add(series);
            }
        }

        #endregion
    }
}
