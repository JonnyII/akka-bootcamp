﻿using Akka.Util;

using DevExpress.XtraCharts;

namespace ChartApp;

/// <summary>
///     Helper class for creating random data for chart plots
/// </summary>
public static class ChartDataHelper
{
    public static Series RandomSeries(string seriesName, ViewType type = ViewType.Line, int points = 100)
    {
        var series = new Series(seriesName, type);
        foreach (var i in Enumerable.Range(0, points))
        {
            var rng = ThreadLocalRandom.Current.NextDouble();
            series.Points.Add(new(i, (2.0 * Math.Sin(rng)) + Math.Sin(rng / 4.5)));
        }

        //series.BorderWidth = 3;
        return series;
    }
}
