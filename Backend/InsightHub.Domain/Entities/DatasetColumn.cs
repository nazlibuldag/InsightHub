using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Domain.Common;
using InsightHub.Domain.Enums;

namespace InsightHub.Domain.Entities;

public class DatasetColumn : BaseEntity
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public DataType DataType { get; set; } = DataType.Unknown;

    public int NullCount { get; set; }

    public int UniqueCount { get; set; }

    public Dataset Dataset { get; set; } = null!;

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    public double? AverageValue { get; set; }

    public double? MedianValue { get; set; }

    public double? StandardDeviation { get; set; }

    public ICollection<DatasetColumnValue> Values { get; set; }
    = new List<DatasetColumnValue>();
}
