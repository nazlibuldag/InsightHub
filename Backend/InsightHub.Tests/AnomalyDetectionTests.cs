using System.Collections.Generic;
using InsightHub.Infrastructure.Services;
using Xunit;

namespace InsightHub.Tests;

public class AnomalyDetectionTests
{
    [Fact]
    public void DetectColumnAnomalies_ShouldIdentifyHighSpikeAnomalies()
    {
        // Arrange
        var service = new AnomalyDetectionService();
        var values = new List<double> { 10.0, 10.2, 9.8, 10.1, 10.3, 100.0, 9.9, 10.0 };

        // Act
        var result = service.DetectColumnAnomalies(values, "temperature", zThreshold: 2.0);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("temperature", result.ColumnName);
        Assert.True(result.AnomalyCount >= 1);
        Assert.Contains(result.AnomalousRows, r => r.RowNumber == 6 && r.Value == 100.0);
    }
}
