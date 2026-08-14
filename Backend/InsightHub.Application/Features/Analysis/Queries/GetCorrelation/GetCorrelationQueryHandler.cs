using System.Text.Json;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelation;

public class GetCorrelationQueryHandler
    : IRequestHandler<GetCorrelationQuery, GetCorrelationResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetCorrelationQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetCorrelationResponse> Handle(
        GetCorrelationQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var values = new List<(double X, double Y)>();

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            if (!document.RootElement.TryGetProperty(
                    request.Column1,
                    out var value1))
            {
                continue;
            }

            if (!document.RootElement.TryGetProperty(
                    request.Column2,
                    out var value2))
            {
                continue;
            }

            if (!double.TryParse(
                    value1.GetString(),
                    out var x))
            {
                continue;
            }

            if (!double.TryParse(
                    value2.GetString(),
                    out var y))
            {
                continue;
            }

            values.Add((x, y));
        }

        if (values.Count < 2)
        {
            throw new Exception(
                "Korelasyon hesaplamak için yeterli veri bulunamadı.");
        }

        var averageX = values.Average(x => x.X);
        var averageY = values.Average(x => x.Y);

        var numerator = values.Sum(
            x => (x.X - averageX) * (x.Y - averageY));

        var denominatorX = Math.Sqrt(
            values.Sum(x => Math.Pow(x.X - averageX, 2)));

        var denominatorY = Math.Sqrt(
            values.Sum(x => Math.Pow(x.Y - averageY, 2)));

        if (denominatorX == 0 || denominatorY == 0)
        {
            throw new Exception(
                "Korelasyon hesaplanamadı. Kolonlardan biri sabit değere sahip.");
        }

        var correlation =
            numerator / (denominatorX * denominatorY);

        return new GetCorrelationResponse
        {
            Column1 = request.Column1,
            Column2 = request.Column2,
            Correlation = correlation
        };
    }
}