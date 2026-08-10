using InsightHub.Application.Common;

namespace InsightHub.Application.Interfaces;

public interface IColumnAnalysisService
{
    Task<ColumnAnalysisResult> AnalyzeAsync(
        string filePath,
        Guid datasetId);
}