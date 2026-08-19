using System;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.ExportDataset;

public class ExportDatasetQuery : IRequest<ExportDatasetResponse>
{
    public Guid DatasetId { get; set; }

    public string Format { get; set; } = "csv";
}

public class ExportDatasetResponse
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}
