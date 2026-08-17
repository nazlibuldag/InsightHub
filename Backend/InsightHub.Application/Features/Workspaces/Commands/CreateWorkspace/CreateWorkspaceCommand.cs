using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(
    Guid OwnerId,
    string Name,
    string Description
) : IRequest<WorkspaceDto>;

public record WorkspaceDto(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    int MemberCount,
    DateTime CreatedDate
);

public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, WorkspaceDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public CreateWorkspaceCommandHandler(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<WorkspaceDto> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            CreatedDate = DateTime.UtcNow
        };

        var ownerMember = new WorkspaceMember
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspace.Id,
            UserId = request.OwnerId,
            Role = "Owner",
            CreatedDate = DateTime.UtcNow
        };

        workspace.Members.Add(ownerMember);

        var result = await _workspaceRepository.AddAsync(workspace, cancellationToken);

        return new WorkspaceDto(
            result.Id,
            result.Name,
            result.Description,
            result.OwnerId,
            result.Members.Count,
            result.CreatedDate
        );
    }
}
