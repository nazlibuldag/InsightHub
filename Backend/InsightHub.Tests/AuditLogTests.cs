using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.AuditLogs.Queries.GetRecentAuditLogs;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using Moq;
using Xunit;

namespace InsightHub.Tests;

public class AuditLogTests
{
    [Fact]
    public async Task GetRecentAuditLogsQueryHandler_ShouldReturnRecentLogs()
    {
        // Arrange
        var mockAuditRepo = new Mock<IAuditLogRepository>();
        var userId = Guid.NewGuid();

        var logs = new List<AuditLog>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserEmail = "admin@insighthub.com",
                Action = "UPLOAD_DATASET",
                EntityName = "Dataset",
                EntityId = Guid.NewGuid().ToString(),
                IpAddress = "127.0.0.1",
                Details = "Uploaded quarterly sales CSV",
                Timestamp = DateTime.UtcNow
            }
        };

        mockAuditRepo.Setup(r => r.GetRecentLogsAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var handler = new GetRecentAuditLogsQueryHandler(mockAuditRepo.Object);

        // Act
        var result = await handler.Handle(new GetRecentAuditLogsQuery(50), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("UPLOAD_DATASET", result[0].Action);
        Assert.Equal("admin@insighthub.com", result[0].UserEmail);
    }
}
