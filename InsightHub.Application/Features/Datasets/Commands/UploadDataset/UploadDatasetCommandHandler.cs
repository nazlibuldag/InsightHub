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
    private readonly IDatasetColumnValueRepository _datasetColumnValueRepository;

    public UploadDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IFileStorageService fileStorageService,
        ICsvReaderService csvReaderService,
        IExcelReaderService excelReaderService,
        IColumnAnalysisService columnAnalysisService,
        IExcelColumnAnalysisService excelColumnAnalysisService,
        IDatasetColumnRepository datasetColumnRepository,
        IDatasetColumnValueRepository datasetColumnValueRepository)
    {
        _datasetRepository = datasetRepository;
        _fileStorageService = fileStorageService;
        _csvReaderService = csvReaderService;
        _excelReaderService = excelReaderService;
        _columnAnalysisService = columnAnalysisService;
        _excelColumnAnalysisService = excelColumnAnalysisService;
        _datasetColumnRepository = datasetColumnRepository;
        _datasetColumnValueRepository = datasetColumnValueRepository;
    }

    public async Task<Guid> Handle(
        UploadDatasetCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Dosyayı kaydet
        var fileName = await _fileStorageService.SaveFileAsync(
            request.File,
            cancellationToken);

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            fileName);

        // 2. Dosya bilgilerini oku
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

        // 3. Dataset oluştur
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

        // 4. CSV analizi
        if (extension == ".csv")
        {
            var analysisResult =
                await _columnAnalysisService.AnalyzeAsync(
                    filePath,
                    dataset.Id);

            // 5. Kolonları veritabanına kaydet
            await _datasetColumnRepository.AddRangeAsync(
                analysisResult.Columns,
                cancellationToken);

            // 6. DatasetColumnValue kayıtlarını oluştur
            var columnValues = new List<DatasetColumnValue>();

            foreach (var column in analysisResult.Columns)
            {
                if (!analysisResult.ColumnValues.TryGetValue(
                        column.ColumnName,
                        out var values))
                {
                    continue;
                }

                // String kolonlar için frekans bilgisi
                if (column.DataType == Domain.Enums.DataType.String)
                {
                    var groups = values
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .GroupBy(v => v);

                    foreach (var group in groups)
                    {
                        columnValues.Add(
                            new DatasetColumnValue
                            {
                                DatasetColumnId = column.Id,
                                Value = group.Key,
                                Count = group.Count()
                            });
                    }
                }
            }

            // 7. Value kayıtlarını veritabanına kaydet
            if (columnValues.Any())
            {
                await _datasetColumnValueRepository.AddRangeAsync(
                    columnValues,
                    cancellationToken);
            }
        }
        // 8. Excel analizi
        else if (extension == ".xlsx")
        {
            var columns =
                await _excelColumnAnalysisService.AnalyzeAsync(
                    filePath,
                    dataset.Id,
                    cancellationToken);

            await _datasetColumnRepository.AddRangeAsync(
                columns,
                cancellationToken);
        }

        return dataset.Id;
    }
}