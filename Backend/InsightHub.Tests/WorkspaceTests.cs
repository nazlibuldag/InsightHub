using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.Workspaces.Commands.CreateWorkspace;
using InsightHub.Application.Features.Workspaces.Queries.GetUserWorkspaces;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using Moq;
using Xunit;

namespace InsightHub.Tests;

public class WorkspaceTests
{
    [Fact]
    public async Task CreateWorkspaceCommandHandler_ShouldCreateWorkspaceAndAddOwner()
    {
        // Arrange
        var mockWorkspaceRepo = new Mock<IWorkspaceRepository>();
        var ownerId = Guid.NewGuid();

        mockWorkspaceRepo.Setup(r => r.AddAsync(It.IsAny<Workspace>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workspace ws, CancellationToken ct) => ws);

        var handler = new CreateWorkspaceCommandHandler(mockWorkspaceRepo.Object);
        var command = new CreateWorkspaceCommand(ownerId, "Finance Team", "Finance analytics workspace");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Finance Team", result.Name);
        Assert.Equal("Finance analytics workspace", result.Description);
        Assert.Equal(ownerId, result.OwnerId);
    }

    [Fact]
    public async Task GetUserWorkspacesQueryHandler_ShouldReturnUserWorkspaces()
    {
        // Arrange
        var mockWorkspaceRepo = new Mock<IWorkspaceRepository>();
        var userId = Guid.NewGuid();

        var list = new List<Workspace>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Data Science Team",
                Description = "ML & AI workspace",
                OwnerId = userId,
                Members = new List<WorkspaceMember>
                {
                    new() { UserId = userId, Role = "Owner" }
                }
            }
        };

        mockWorkspaceRepo.Setup(r => r.GetUserWorkspacesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new GetUserWorkspacesQueryHandler(mockWorkspaceRepo.Object);

        // Act
        var result = await handler.Handle(new GetUserWorkspacesQuery(userId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Data Science Team", result[0].Name);
        Assert.Equal(userId, result[0].OwnerId);
    }
}
