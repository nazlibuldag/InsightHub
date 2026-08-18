using System.Threading;
using System.Threading.Tasks;

namespace InsightHub.Application.Interfaces;

public interface IAiAnalysisService
{
    Task<string> GenerateDatasetInsightsAsync(
        string datasetName,
        int totalRows,
        int totalColumns,
        string columnSummaryJson,
        string statsSummaryJson,
        CancellationToken cancellationToken = default);
}
