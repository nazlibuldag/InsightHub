using System.Collections.Generic;

namespace InsightHub.Application.Interfaces;

public record CleaningResultDto(
    string DatasetId,
    int CleanedRowCount,
    int FixedMissingValuesCount,
    List<string> AppliedStrategies
);

public interface IDataCleaningService
{
    List<double> ImputeNumericColumn(List<double?> rawValues, string strategy = "MEAN");
}
