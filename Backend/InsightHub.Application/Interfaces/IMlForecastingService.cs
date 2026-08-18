using System;
using System.Collections.Generic;

namespace InsightHub.Application.Interfaces;

public class ForecastingResultDto
{
    public string TargetColumn { get; set; } = string.Empty;
    public double Slope { get; set; }
    public double Intercept { get; set; }
    public double RSquared { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public List<double> HistoricalValues { get; set; } = new();
    public List<double> ForecastedValues { get; set; } = new();
}

public interface IMlForecastingService
{
    ForecastingResultDto ForecastColumnTrend(List<double> values, string columnName, int stepsAhead = 5);
}
