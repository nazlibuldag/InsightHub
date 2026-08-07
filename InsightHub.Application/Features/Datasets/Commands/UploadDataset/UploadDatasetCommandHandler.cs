using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UploadDataset;

public class UploadDatasetCommandHandler
    : IRequestHandler<UploadDatasetCommand, Guid>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICsvReaderService _csvReaderService;
    private readonly IExcelReaderService _excelReaderService;

    private readonly IColumnAnalysisService _columnAnalysisService;
    private readonly IExcelColumnAnalysisService _excelColumnAnalysisService;

    private readonly IDatasetColumnRepository _datasetColumnRepository;

    public UploadDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IFileStorageService fileStorageService,
        ICsvReaderService csvReaderService,
        IExcelReaderService excelReaderService,
        IColumnAnalysisService columnAnalysisService,
        IExcelColumnAnalysisService excelColumnAnalysisService,
        IDatasetColumnRepository datasetColumnRepository)
    {
        _datasetRepository = datasetRepository;
        _fileStorageService = fileStorageService;
        _csvReaderService = csvReaderService;
        _excelReaderService = excelReaderService;
        _columnAnalysisService = columnAnalysisService;
        _excelColumnAnalysisService = excelColumnAnalysisService;
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

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            fileName);

        (int TotalRows, int TotalColumns) fileInfo;

        var extension = Path.GetExtension(fileName).ToLower();

        if (extension == ".csv")
        {
            fileInfo = await _csvReaderService.ReadCsvInfoAsync(
                filePath,
                cancellationToken);
        }
        else if (extension == ".xlsx")
        {
            fileInfo = await _excelReaderService.ReadExcelInfoAsync(
                filePath,
                cancellationToken);
        }
        else
        {
            throw new Exception("Desteklenmeyen dosya formatı.");
        }

        var dataset = new Dataset
        {
            Name = request.Name,
            Description = request.Description,
            FileName = fileName,
            TotalRows = fileInfo.TotalRows,
            TotalColumns = fileInfo.TotalColumns
        };

        await _datasetRepository.AddAsync(
            dataset,
            cancellationToken);

        List<DatasetColumn> columns;

        if (extension == ".csv")
        {
            columns = await _columnAnalysisService.AnalyzeAsync(
                filePath,
                dataset.Id);
        }
        else
        {
            columns = await _excelColumnAnalysisService.AnalyzeAsync(
                filePath,
                dataset.Id,
                cancellationToken);
        }

        await _datasetColumnRepository.AddRangeAsync(
            columns,
            cancellationToken);

        return dataset.Id;
    }
}