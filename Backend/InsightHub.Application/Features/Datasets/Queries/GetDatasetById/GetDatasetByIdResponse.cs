using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetById;

public class GetDatasetByIdResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TotalRows { get; set; }

    public int TotalColumns { get; set; }

    public DateTime UploadedAt { get; set; }

    public List<DatasetColumnResponse> Columns { get; set; } = [];
}