using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;


namespace InsightHub.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    public double GetMin(List<double> values)
    {
        return values.Min();
    }

    public double GetMax(List<double> values)
    {
        return values.Max();
    }

    public double GetAverage(List<double> values)
    {
        return values.Average();
    }

    public double GetMedian(List<double> values)
    {
        var ordered = values.OrderBy(x => x).ToList();

        int count = ordered.Count;

        if (count % 2 == 0)
        {
            return (ordered[count / 2 - 1] + ordered[count / 2]) / 2;
        }

        return ordered[count / 2];
    }

    public double GetStandardDeviation(List<double> values)
    {
        var average = values.Average();

        var sum = values.Sum(v => Math.Pow(v - average, 2));

        return Math.Sqrt(sum / values.Count);
    }
}