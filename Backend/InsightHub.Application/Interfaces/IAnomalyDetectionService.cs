using System.Collections.Generic;

namespace InsightHub.Application.Interfaces;

public record AnomalousRowDto(
    int RowNumber,
    double Value,
    double ZScore,
    string AnomalyType // "HIGH_OUTLIER", "LOW_OUTLIER", "Z_SCORE_SPIKE"
);

public record ColumnAnomalyReportDto(
    string ColumnName,
    int TotalValues,
    int AnomalyCount,
    double AnomalyPercentage,
    double Mean,
    double StandardDeviation,
    List<AnomalousRowDto> AnomalousRows
);

public record DatasetAnomalyReportDto(
    string DatasetId,
    List<ColumnAnomalyReportDto> ColumnReports
);

public interface IAnomalyDetectionService
{
    ColumnAnomalyReportDto DetectColumnAnomalies(List<double> values, string columnName, double zThreshold = 2.5);
}
