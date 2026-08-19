using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.ExportDatasetPdf;

public class ExportDatasetPdfQuery : IRequest<ExportDatasetPdfResponse>
{
    public Guid DatasetId { get; set; }
}

public class ExportDatasetPdfResponse
{
    public byte[] FileContents { get; set; } = Array.Empty<byte>();

    public string FileName { get; set; } = string.Empty;
}

public class ExportDatasetPdfQueryHandler : IRequestHandler<ExportDatasetPdfQuery, ExportDatasetPdfResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IPdfReportService _pdfReportService;

    public ExportDatasetPdfQueryHandler(
        IDatasetRepository datasetRepository,
        IPdfReportService pdfReportService)
    {
        _datasetRepository = datasetRepository;
        _pdfReportService = pdfReportService;
    }

    public async Task<ExportDatasetPdfResponse> Handle(ExportDatasetPdfQuery request, CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new KeyNotFoundException("Dataset bulunamadı.");
        }

        var pdfBytes = await _pdfReportService.GenerateDatasetPdfReportAsync(request.DatasetId, cancellationToken);

        var safeName = string.Join("_", dataset.Name.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"{safeName}_Report_{DateTime.Now:yyyyMMdd}.pdf";

        return new ExportDatasetPdfResponse
        {
            FileContents = pdfBytes,
            FileName = fileName
        };
    }
}
