using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightHub.Domain.Common;
using InsightHub.Domain.Enums;

namespace InsightHub.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Analyst;

    public bool IsActive { get; set; } = true;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<Dataset> Datasets { get; set; } = new List<Dataset>();

    public ICollection<SavedAnalysis> SavedAnalyses { get; set; } = new List<SavedAnalysis>();
}