using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace InsightHub.API.Hubs;

public class AnalysisHub : Hub
{
    public async Task JoinDatasetGroup(string datasetId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, datasetId);
    }

    public async Task LeaveDatasetGroup(string datasetId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, datasetId);
    }
}
