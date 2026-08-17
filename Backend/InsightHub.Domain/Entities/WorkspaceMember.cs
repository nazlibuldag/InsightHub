using System;
using InsightHub.Domain.Common;

namespace InsightHub.Domain.Entities;

public class WorkspaceMember : BaseEntity
{
    public Guid WorkspaceId { get; set; }

    public Workspace? Workspace { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Role { get; set; } = "Analyst"; // Owner, Admin, Analyst, Viewer
}
