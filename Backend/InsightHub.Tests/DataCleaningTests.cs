using System.Collections.Generic;
using InsightHub.Infrastructure.Services;
using Xunit;

namespace InsightHub.Tests;

public class DataCleaningTests
{
    [Fact]
    public void ImputeNumericColumn_ShouldImputeMissingValuesUsingMean()
    {
        // Arrange
        var service = new DataCleaningService();
        var rawValues = new List<double?> { 10.0, null, 30.0, 40.0 };

        // Act
        var result = service.ImputeNumericColumn(rawValues, strategy: "MEAN");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Equal(26.6667, result[1], precision: 2); // Mean of 10, 30, 40 is 26.6667
    }

    [Fact]
    public void ImputeNumericColumn_ShouldImputeMissingValuesUsingZero()
    {
        // Arrange
        var service = new DataCleaningService();
        var rawValues = new List<double?> { 10.0, null, 30.0 };

        // Act
        var result = service.ImputeNumericColumn(rawValues, strategy: "ZERO");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.0, result[1]);
    }
}
