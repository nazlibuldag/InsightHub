using System;
using System.Collections.Generic;
using System.Linq;
using InsightHub.Application.Interfaces;

namespace InsightHub.Infrastructure.Services;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    public ColumnAnomalyReportDto DetectColumnAnomalies(List<double> values, string columnName, double zThreshold = 2.5)
    {
        if (values == null || values.Count < 3)
        {
            return new ColumnAnomalyReportDto(
                columnName,
                values?.Count ?? 0,
                0,
                0.0,
                0.0,
                0.0,
                new List<AnomalousRowDto>()
            );
        }

        var mean = values.Average();
        var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        var stdDev = Math.Sqrt(variance);

        if (stdDev == 0)
        {
            return new ColumnAnomalyReportDto(
                columnName,
                values.Count,
                0,
                0.0,
                mean,
                0.0,
                new List<AnomalousRowDto>()
            );
        }

        var anomalousRows = new List<AnomalousRowDto>();

        for (int i = 0; i < values.Count; i++)
        {
            var val = values[i];
            var zScore = (val - mean) / stdDev;

            if (Math.Abs(zScore) >= zThreshold)
            {
                var type = zScore > 0 ? "HIGH_OUTLIER" : "LOW_OUTLIER";
                anomalousRows.Add(new AnomalousRowDto(
                    i + 1,
                    val,
                    Math.Round(zScore, 2),
                    type
                ));
            }
        }

        var anomalyCount = anomalousRows.Count;
        var percentage = Math.Round(((double)anomalyCount / values.Count) * 100, 2);

        return new ColumnAnomalyReportDto(
            columnName,
            values.Count,
            anomalyCount,
            percentage,
            Math.Round(mean, 4),
            Math.Round(stdDev, 4),
            anomalousRows
        );
    }
}
