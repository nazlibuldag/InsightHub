using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.Analysis.Queries.GetDescriptiveStatistics;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using Moq;
using Xunit;

namespace InsightHub.Tests;

public class DescriptiveStatisticsTests
{
    [Fact]
    public async Task Handle_ShouldCalculateCorrectStatistics_ForValidNumericColumn()
    {
        // Arrange
        var mockRepo = new Mock<IDatasetRowRepository>();
        var datasetId = Guid.NewGuid();

        var mockRows = new List<DatasetRow>
        {
            new() { DatasetId = datasetId, RowNumber = 1, Data = "{\"val\": 10}" },
            new() { DatasetId = datasetId, RowNumber = 2, Data = "{\"val\": 20}" },
            new() { DatasetId = datasetId, RowNumber = 3, Data = "{\"val\": 30}" },
            new() { DatasetId = datasetId, RowNumber = 4, Data = "{\"val\": 40}" },
            new() { DatasetId = datasetId, RowNumber = 5, Data = "{\"val\": 50}" }
        };

        mockRepo.Setup(r => r.GetByDatasetIdAsync(datasetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockRows);

        var handler = new GetDescriptiveStatisticsQueryHandler(mockRepo.Object);
        var query = new GetDescriptiveStatisticsQuery { DatasetId = datasetId, ColumnName = "val" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.Equal(30, result.Mean);
        Assert.Equal(30, result.Median);
        Assert.Equal(10, result.Min);
        Assert.Equal(50, result.Max);
        Assert.Equal(40, result.Range);
    }
}
