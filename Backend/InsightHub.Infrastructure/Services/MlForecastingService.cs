using System;
using System.Collections.Generic;
using System.Linq;
using InsightHub.Application.Interfaces;

namespace InsightHub.Infrastructure.Services;

public class MlForecastingService : IMlForecastingService
{
    public ForecastingResultDto ForecastColumnTrend(List<double> values, string columnName, int stepsAhead = 5)
    {
        if (values == null || values.Count < 2)
        {
            return new ForecastingResultDto
            {
                TargetColumn = columnName,
                TrendDirection = "Yetersiz Veri"
            };
        }

        int n = values.Count;
        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;

        for (int i = 0; i < n; i++)
        {
            double x = i + 1;
            double y = values[i];

            sumX += x;
            sumY += y;
            sumXY += x * y;
            sumX2 += x * x;
            sumY2 += y * y;
        }

        double denominator = (n * sumX2 - sumX * sumX);
        if (Math.Abs(denominator) < 1e-9)
        {
            return new ForecastingResultDto
            {
                TargetColumn = columnName,
                TrendDirection = "Yatay (Sabit)",
                HistoricalValues = values,
                ForecastedValues = Enumerable.Repeat(values.Average(), stepsAhead).ToList()
            };
        }

        double slope = (n * sumXY - sumX * sumY) / denominator;
        double intercept = (sumY - slope * sumX) / n;

        double rNumerator = (n * sumXY - sumX * sumY);
        double rDenominator = Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));
        double r = Math.Abs(rDenominator) > 1e-9 ? rNumerator / rDenominator : 0;
        double rSquared = r * r;

        string trendDirection = slope > 0.01 ? "📈 Yükseliş Eğilimi (Upward Trend)" :
                                slope < -0.01 ? "📉 Düşüş Eğilimi (Downward Trend)" :
                                "➡️ Yatay / Sabit (Stable)";

        var forecasted = new List<double>();
        for (int i = 1; i <= stepsAhead; i++)
        {
            double futureX = n + i;
            double futureY = slope * futureX + intercept;
            forecasted.Add(Math.Round(futureY, 2));
        }

        return new ForecastingResultDto
        {
            TargetColumn = columnName,
            Slope = Math.Round(slope, 4),
            Intercept = Math.Round(intercept, 4),
            RSquared = Math.Round(rSquared, 4),
            TrendDirection = trendDirection,
            HistoricalValues = values.Select(v => Math.Round(v, 2)).ToList(),
            ForecastedValues = forecasted
        };
    }
}
