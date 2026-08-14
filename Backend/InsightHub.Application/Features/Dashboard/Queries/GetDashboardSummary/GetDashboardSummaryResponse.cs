using InsightHub.Domain.Enums;

namespace InsightHub.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryResponse
{
    public string DatasetName { get; set; } = string.Empty;

    public int TotalRows { get; set; }

    public int TotalColumns { get; set; }

    public int NumericColumns { get; set; }

    public int StringColumns { get; set; }

    public int DateColumns { get; set; }

    public int BooleanColumns { get; set; }

    public int TotalMissingValues { get; set; }

    public List<DashboardColumnResponse> Columns { get; set; } = [];
}

public class DashboardColumnResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public DataType DataType { get; set; }
}