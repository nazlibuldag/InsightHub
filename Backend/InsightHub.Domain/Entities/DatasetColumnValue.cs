using InsightHub.Domain.Common;

namespace InsightHub.Domain.Entities;

public class DatasetColumnValue : BaseEntity
{
    public Guid DatasetColumnId { get; set; }

    public string Value { get; set; } = string.Empty;

    public int Count { get; set; }

    public DatasetColumn DatasetColumn { get; set; } = null!;
}