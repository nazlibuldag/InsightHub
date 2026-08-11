namespace InsightHub.Application.Features.Datasets.Commands.UpdateDataset;

public class UpdateDatasetRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}