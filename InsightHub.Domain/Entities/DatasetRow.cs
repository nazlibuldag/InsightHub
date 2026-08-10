using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsightHub.Domain.Entities;

public class DatasetRow
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DatasetId { get; set; }

    public int RowNumber { get; set; }

    public string Data { get; set; } = string.Empty;

    [ForeignKey(nameof(DatasetId))]
    public Dataset Dataset { get; set; } = null!;
}