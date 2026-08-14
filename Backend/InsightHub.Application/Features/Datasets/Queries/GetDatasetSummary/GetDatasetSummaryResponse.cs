using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetSummary;

public class GetDatasetSummaryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TotalRows { get; set; }

    public int TotalColumns { get; set; }

    public int NumericColumns { get; set; }

    public int StringColumns { get; set; }

    public int BooleanColumns { get; set; }

    public int DateColumns { get; set; }

    public int TotalMissingValues { get; set; }
}