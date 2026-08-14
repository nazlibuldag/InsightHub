using System.Text.Json;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelationMatrix;

public class GetCorrelationMatrixQueryHandler
    : IRequestHandler<GetCorrelationMatrixQuery, GetCorrelationMatrixResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetCorrelationMatrixQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetCorrelationMatrixResponse> Handle(
        GetCorrelationMatrixQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        if (!rows.Any())
        {
            throw new Exception("Dataset'te veri bulunamadı.");
        }

        var parsedRows = new List<Dictionary<string, double>>();

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            var numericValues = new Dictionary<string, double>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (double.TryParse(
                        property.Value.GetString(),
                        out var value))
                {
                    numericValues[property.Name] = value;
                }
            }

            if (numericValues.Any())
            {
                parsedRows.Add(numericValues);
            }
        }

        var columns = parsedRows
            .SelectMany(x => x.Keys)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var matrix = new List<List<double>>();

        foreach (var column1 in columns)
        {
            var rowValues = new List<double>();

            foreach (var column2 in columns)
            {
                var pairs = parsedRows
                    .Where(x =>
                        x.ContainsKey(column1) &&
                        x.ContainsKey(column2))
                    .Select(x => (X: x[column1], Y: x[column2]))
                    .ToList();

                if (pairs.Count < 2)
                {
                    rowValues.Add(0);
                    continue;
                }

                var averageX = pairs.Average(x => x.X);
                var averageY = pairs.Average(x => x.Y);

                var numerator = pairs.Sum(
                    x => (x.X - averageX) * (x.Y - averageY));

                var denominatorX = Math.Sqrt(
                    pairs.Sum(x =>
                        Math.Pow(x.X - averageX, 2)));

                var denominatorY = Math.Sqrt(
                    pairs.Sum(x =>
                        Math.Pow(x.Y - averageY, 2)));

                if (denominatorX == 0 || denominatorY == 0)
                {
                    rowValues.Add(0);
                    continue;
                }

                var correlation =
                    numerator / (denominatorX * denominatorY);

                rowValues.Add(correlation);
            }

            matrix.Add(rowValues);
        }

        return new GetCorrelationMatrixResponse
        {
            Columns = columns,
            Matrix = matrix
        };
    }
}