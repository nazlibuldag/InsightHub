using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UploadDataset;

public class UploadDatasetCommandHandler : IRequestHandler<UploadDatasetCommand, Guid>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICsvReaderService _csvReaderService;

    private readonly IColumnAnalysisService _columnAnalysisService;
    private readonly IDatasetColumnRepository _datasetColumnRepository;

    public UploadDatasetCommandHandler(
     IDatasetRepository datasetRepository,
     IFileStorageService fileStorageService,
     ICsvReaderService csvReaderService,
     IColumnAnalysisService columnAnalysisService,
     IDatasetColumnRepository datasetColumnRepository)
    {
        _datasetRepository = datasetRepository;
        _fileStorageService = fileStorageService;
        _csvReaderService = csvReaderService;
        _columnAnalysisService = columnAnalysisService;
        _datasetColumnRepository = datasetColumnRepository;
    }

    public async Task<Guid> Handle(
        UploadDatasetCommand request,
        CancellationToken cancellationToken)
    {
        // Dosyayı kaydet
        var fileName = await _fileStorageService.SaveFileAsync(
            request.File,
            cancellationToken);

        // Burayı birazdan düzenleyeceğiz
        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            fileName);

        // CSV bilgilerini oku
        var csvInfo = await _csvReaderService.ReadCsvInfoAsync(
            filePath,
            cancellationToken);

        // Dataset oluştur
        var dataset = new Dataset
        {
            Name = request.Name,
            Description = request.Description,
            FileName = fileName,
            TotalRows = csvInfo.TotalRows,
            TotalColumns = csvInfo.TotalColumns
        };

        await _datasetRepository.AddAsync(dataset, cancellationToken);

        var columns = await _columnAnalysisService.AnalyzeAsync(
    filePath,
    dataset.Id);

        await _datasetColumnRepository.AddRangeAsync(
            columns,
            cancellationToken);

        return dataset.Id;
    }
}