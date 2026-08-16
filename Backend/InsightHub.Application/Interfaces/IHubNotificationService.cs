using System.Threading.Tasks;

namespace InsightHub.Application.Interfaces;

public interface IHubNotificationService
{
    Task SendProgressAsync(string datasetId, int percentage, string stepMessage);

    Task SendAnalysisCompletedAsync(string datasetId, string title, string message);
}
