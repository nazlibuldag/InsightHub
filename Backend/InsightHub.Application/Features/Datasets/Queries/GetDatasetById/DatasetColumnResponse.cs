using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Domain.Enums;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetById;

public class DatasetColumnResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public DataType DataType { get; set; }

    public int NullCount { get; set; }

    public int UniqueCount { get; set; }

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public double? AverageValue { get; set; }

    public double? MedianValue { get; set; }

    public double? StandardDeviation { get; set; }
}