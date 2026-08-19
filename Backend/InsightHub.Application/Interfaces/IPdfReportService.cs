using System;
using System.Threading;
using System.Threading.Tasks;

namespace InsightHub.Application.Interfaces;

public interface IPdfReportService
{
    Task<byte[]> GenerateDatasetPdfReportAsync(
        Guid datasetId,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateSavedAnalysisPdfReportAsync(
        Guid savedAnalysisId,
        CancellationToken cancellationToken = default);
}
