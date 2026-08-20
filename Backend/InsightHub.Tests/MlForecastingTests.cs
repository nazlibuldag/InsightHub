using System.Collections.Generic;
using InsightHub.Infrastructure.Services;
using Xunit;

namespace InsightHub.Tests;

public class MlForecastingTests
{
    [Fact]
    public void ForecastColumnTrend_ShouldCalculateCorrectUpwardTrend()
    {
        // Arrange
        var service = new MlForecastingService();
        var values = new List<double> { 10.0, 20.0, 30.0, 40.0, 50.0 };

        // Act
        var result = service.ForecastColumnTrend(values, "sales", stepsAhead: 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("sales", result.TargetColumn);
        Assert.Equal("📈 Yükseliş Eğilimi (Upward Trend)", result.TrendDirection);
        Assert.True(result.Slope > 0);
        Assert.Equal(3, result.ForecastedValues.Count);
        Assert.Equal(60.0, result.ForecastedValues[0], precision: 1);
        Assert.Equal(70.0, result.ForecastedValues[1], precision: 1);
        Assert.Equal(80.0, result.ForecastedValues[2], precision: 1);
    }

    [Fact]
    public void ForecastColumnTrend_ShouldCalculateCorrectDownwardTrend()
    {
        // Arrange
        var service = new MlForecastingService();
        var values = new List<double> { 100.0, 80.0, 60.0, 40.0, 20.0 };

        // Act
        var result = service.ForecastColumnTrend(values, "cost", stepsAhead: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("📉 Düşüş Eğilimi (Downward Trend)", result.TrendDirection);
        Assert.True(result.Slope < 0);
        Assert.Equal(2, result.ForecastedValues.Count);
        Assert.Equal(0.0, result.ForecastedValues[0], precision: 1);
        Assert.Equal(-20.0, result.ForecastedValues[1], precision: 1);
    }
}
