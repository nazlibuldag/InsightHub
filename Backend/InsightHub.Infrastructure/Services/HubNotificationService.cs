using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace InsightHub.Infrastructure.Services;

public class HubNotificationService<THub> : IHubNotificationService where THub : Hub
{
    private readonly IHubContext<THub> _hubContext;

    public HubNotificationService(IHubContext<THub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendProgressAsync(string datasetId, int percentage, string stepMessage)
    {
        await _hubContext.Clients.Group(datasetId).SendAsync("ReceiveProgress", new
        {
            datasetId,
            percentage,
            stepMessage
        });

        await _hubContext.Clients.All.SendAsync("ReceiveDatasetProgress", datasetId, percentage, stepMessage);

        await _hubContext.Clients.All.SendAsync("ReceiveGlobalProgress", new
        {
            datasetId,
            percentage,
            stepMessage
        });
    }

    public async Task SendAnalysisCompletedAsync(string datasetId, string title, string message)
    {
        await _hubContext.Clients.Group(datasetId).SendAsync("ReceiveAnalysisCompleted", new
        {
            datasetId,
            title,
            message
        });

        await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", new
        {
            datasetId,
            title,
            message
        });
    }
}
