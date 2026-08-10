using InsightHub.Domain.Entities;

namespace InsightHub.Application.Common;

public class ColumnAnalysisResult
{
    public List<DatasetColumn> Columns { get; set; } = new();

    public Dictionary<string, List<string>> ColumnValues { get; set; } = new();
}