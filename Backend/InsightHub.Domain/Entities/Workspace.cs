using System;
using System.Collections.Generic;
using InsightHub.Domain.Common;

namespace InsightHub.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid OwnerId { get; set; }

    public User? Owner { get; set; }

    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
}
