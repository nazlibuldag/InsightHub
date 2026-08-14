using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightHub.Domain.Common;

namespace InsightHub.Domain.Entities;

public class Dataset : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public int TotalRows { get; set; }

    public int TotalColumns { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DatasetColumn> Columns { get; set; } = new List<DatasetColumn>();

    public ICollection<DatasetRow> Rows { get; set; } = new List<DatasetRow>();
}