using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IExcelColumnAnalysisService
{
    Task<List<DatasetColumn>> AnalyzeAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken);
}