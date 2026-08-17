using System;
using InsightHub.Domain.Common;

namespace InsightHub.Domain.Entities;

public class SavedAnalysis : BaseEntity
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid DatasetId { get; set; }

    public Dataset? Dataset { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string AnalysisType { get; set; } = "General";

    public string FilterJson { get; set; } = "{}";

    public string ConfigurationJson { get; set; } = "{}";

    public string ResultJson { get; set; } = "{}";
}
