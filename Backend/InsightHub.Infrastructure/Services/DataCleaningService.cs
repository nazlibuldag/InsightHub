using System;
using System.Collections.Generic;
using System.Linq;
using InsightHub.Application.Interfaces;

namespace InsightHub.Infrastructure.Services;

public class DataCleaningService : IDataCleaningService
{
    public List<double> ImputeNumericColumn(List<double?> rawValues, string strategy = "MEAN")
    {
        if (rawValues == null || rawValues.Count == 0)
        {
            return new List<double>();
        }

        var validValues = rawValues.Where(v => v.HasValue).Select(v => v!.Value).ToList();
        if (validValues.Count == 0)
        {
            return rawValues.Select(_ => 0.0).ToList();
        }

        double fillValue = 0.0;
        if (string.Equals(strategy, "MEDIAN", StringComparison.OrdinalIgnoreCase))
        {
            var sorted = validValues.OrderBy(v => v).ToList();
            fillValue = sorted[sorted.Count / 2];
        }
        else if (string.Equals(strategy, "ZERO", StringComparison.OrdinalIgnoreCase))
        {
            fillValue = 0.0;
        }
        else // Default MEAN
        {
            fillValue = validValues.Average();
        }

        var result = new List<double>();
        double lastValid = fillValue;

        foreach (var val in rawValues)
        {
            if (val.HasValue)
            {
                result.Add(val.Value);
                lastValid = val.Value;
            }
            else
            {
                if (string.Equals(strategy, "FORWARD_FILL", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(lastValid);
                }
                else
                {
                    result.Add(Math.Round(fillValue, 4));
                }
            }
        }

        return result;
    }
}
