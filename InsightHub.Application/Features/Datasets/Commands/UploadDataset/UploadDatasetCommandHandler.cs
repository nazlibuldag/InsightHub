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
    private readonly IDatasetRowService _datasetRowService;
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IExcelDatasetRowService _excelDatasetRowService;

    public UploadDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IFileStorageService fileStorageService,
        ICsvReaderService csvReaderService,
        IExcelReaderService excelReaderService,
        IColumnAnalysisService columnAnalysisService,
        IExcelColumnAnalysisService excelColumnAnalysisService,
        IDatasetColumnRepository datasetColumnRepository,
        IDatasetColumnValueRepository datasetColumnValueRepository,
        IDatasetRowService datasetRowService,
        IDatasetRowRepository datasetRowRepository,
        IExcelDatasetRowService excelDatasetRowService)
    {
        _datasetRepository = datasetRepository;
        _fileStorageService = fileStorageService;
        _csvReaderService = csvReaderService;
        _excelReaderService = excelReaderService;
        _columnAnalysisService = columnAnalysisService;
        _excelColumnAnalysisService = excelColumnAnalysisService;
        _datasetColumnRepository = datasetColumnRepository;
        _datasetColumnValueRepository = datasetColumnValueRepository;
        _datasetRowService = datasetRowService;
        _datasetRowRepository = datasetRowRepository;
        _excelDatasetRowService = excelDatasetRowService;
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

        // 2. Dosya uzantısını kontrol et
        var extension = Path.GetExtension(fileName)
            .ToLowerInvariant();

        // 3. Dosya bilgilerini oku
        (int TotalRows, int TotalColumns) fileInfo;

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
            throw new Exception(
                "Desteklenmeyen dosya formatı.");
        }

        // 4. Dataset oluştur
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

        // 5. CSV analizi
        if (extension == ".csv")
        {
            var analysisResult =
                await _columnAnalysisService.AnalyzeAsync(
                    filePath,
                    dataset.Id);

            // Kolonları kaydet
            await _datasetColumnRepository.AddRangeAsync(
                analysisResult.Columns,
                cancellationToken);

            // DatasetColumnValue kayıtlarını oluştur
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

            // Value kayıtlarını kaydet
            if (columnValues.Any())
            {
                await _datasetColumnValueRepository.AddRangeAsync(
                    columnValues,
                    cancellationToken);
            }

            var rows = await _datasetRowService.ReadRowsAsync(
    filePath,
    dataset.Id,
    cancellationToken);

            if (rows.Any())
            {
                await _datasetRowRepository.AddRangeAsync(
                    rows,
                    cancellationToken);
            }
        }

        else if (extension == ".xlsx")
        {
            var columns =
                await _excelColumnAnalysisService.AnalyzeAsync(
                    filePath,
                    dataset.Id,
                    cancellationToken);

            // Önce kolonları DB'ye kaydet
            await _datasetColumnRepository.AddRangeAsync(
                columns,
                cancellationToken);

            // Excel satırlarını oku
            var rows =
                await _excelDatasetRowService.ReadRowsAsync(
                    filePath,
                    dataset.Id,
                    cancellationToken);

            await _datasetRowRepository.AddRangeAsync(
                rows,
                cancellationToken);

            // Kategorik kolonların değerlerini oluştur
            var columnValues = new List<DatasetColumnValue>();

            foreach (var column in columns)
            {
                if (column.DataType != Domain.Enums.DataType.String)
                    continue;

                foreach (var row in rows)
                {
                    using var document =
                        System.Text.Json.JsonDocument.Parse(row.Data);

                    if (!document.RootElement.TryGetProperty(
                            column.ColumnName,
                            out var property))
                    {
                        continue;
                    }

                    var value = property.GetString();

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    var existingValue = columnValues.FirstOrDefault(
                        x => x.DatasetColumnId == column.Id &&
                             x.Value == value);

                    if (existingValue == null)
                    {
                        columnValues.Add(
                            new DatasetColumnValue
                            {
                                DatasetColumnId = column.Id,
                                Value = value,
                                Count = 1
                            });
                    }
                    else
                    {
                        existingValue.Count++;
                    }
                }
            }

            // Value'ları DB'ye kaydet
            if (columnValues.Any())
            {
                await _datasetColumnValueRepository.AddRangeAsync(
                    columnValues,
                    cancellationToken);
            }
        }


        // 8. Dataset ID'sini döndür
        return dataset.Id;
    }
}