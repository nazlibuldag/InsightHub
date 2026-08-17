using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.Workspaces.Commands.CreateWorkspace;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Workspaces.Queries.GetUserWorkspaces;

public record GetUserWorkspacesQuery(Guid UserId) : IRequest<List<WorkspaceDto>>;

public class GetUserWorkspacesQueryHandler : IRequestHandler<GetUserWorkspacesQuery, List<WorkspaceDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetUserWorkspacesQueryHandler(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<List<WorkspaceDto>> Handle(GetUserWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var list = await _workspaceRepository.GetUserWorkspacesAsync(request.UserId, cancellationToken);
        return list.Select(x => new WorkspaceDto(
            x.Id,
            x.Name,
            x.Description,
            x.OwnerId,
            x.Members.Count,
            x.CreatedDate
        )).ToList();
    }
}
