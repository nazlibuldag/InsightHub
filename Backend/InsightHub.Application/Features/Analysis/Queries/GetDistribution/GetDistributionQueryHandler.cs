using System.Globalization;
using System.Text.Json;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDistribution;

public class GetDistributionQueryHandler
    : IRequestHandler<GetDistributionQuery, GetDistributionResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetDistributionQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetDistributionResponse> Handle(
        GetDistributionQuery request,
        CancellationToken cancellationToken)
    {
        if (request.BinCount < 1)
        {
            throw new Exception(
                "Bin sayısı 1 veya daha büyük olmalıdır.");
        }

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var values = new List<double>();

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            if (!document.RootElement.TryGetProperty(
                    request.ColumnName,
                    out var property))
            {
                continue;
            }

            if (double.TryParse(
                    property.GetString(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var value))
            {
                values.Add(value);
            }
        }

        if (!values.Any())
        {
            throw new Exception(
                "Seçilen kolonda sayısal veri bulunamadı.");
        }

        var minValue = values.Min();
        var maxValue = values.Max();

        if (minValue == maxValue)
        {
            return new GetDistributionResponse
            {
                ColumnName = request.ColumnName,
                MinValue = minValue,
                MaxValue = maxValue,
                BinCount = 1,
                Bins =
                [
                    new DistributionBinResponse
                    {
                        From = minValue,
                        To = maxValue,
                        Count = values.Count
                    }
                ]
            };
        }

        var binWidth =
            (maxValue - minValue) / request.BinCount;

        var bins = new List<DistributionBinResponse>();

        for (var i = 0; i < request.BinCount; i++)
        {
            var from = minValue + (i * binWidth);

            var to = i == request.BinCount - 1
                ? maxValue
                : minValue + ((i + 1) * binWidth);

            var count = values.Count(value =>
                i == request.BinCount - 1
                    ? value >= from && value <= to
                    : value >= from && value < to);

            bins.Add(
                new DistributionBinResponse
                {
                    From = from,
                    To = to,
                    Count = count
                });
        }

        return new GetDistributionResponse
        {
            ColumnName = request.ColumnName,
            MinValue = minValue,
            MaxValue = maxValue,
            BinCount = request.BinCount,
            Bins = bins
        };
    }
}