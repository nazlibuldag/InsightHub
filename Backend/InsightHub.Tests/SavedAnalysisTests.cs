using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.SavedAnalyses.Commands.CreateSavedAnalysis;
using InsightHub.Application.Features.SavedAnalyses.Queries.GetUserSavedAnalyses;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using Moq;
using Xunit;

namespace InsightHub.Tests;

public class SavedAnalysisTests
{
    [Fact]
    public async Task CreateSavedAnalysisCommandHandler_ShouldSaveAndReturnDto()
    {
        // Arrange
        var mockSavedRepo = new Mock<ISavedAnalysisRepository>();
        var mockDatasetRepo = new Mock<IDatasetRepository>();

        var userId = Guid.NewGuid();
        var datasetId = Guid.NewGuid();

        var dataset = new Dataset
        {
            Id = datasetId,
            Name = "Iris Test Dataset"
        };

        mockDatasetRepo.Setup(r => r.GetByIdAsync(datasetId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dataset);

        mockSavedRepo.Setup(r => r.AddAsync(It.IsAny<SavedAnalysis>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SavedAnalysis sa, CancellationToken ct) => sa);

        var handler = new CreateSavedAnalysisCommandHandler(mockSavedRepo.Object, mockDatasetRepo.Object);
        var command = new CreateSavedAnalysisCommand(userId, datasetId, "Test Title", "Test Notes", "General", "{\"chart\":\"line\"}", "{}", "{}");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(datasetId, result.DatasetId);
        Assert.Equal("Iris Test Dataset", result.DatasetName);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Notes", result.Notes);
    }

    [Fact]
    public async Task GetUserSavedAnalysesQueryHandler_ShouldReturnUserAnalyses()
    {
        // Arrange
        var mockSavedRepo = new Mock<ISavedAnalysisRepository>();
        var userId = Guid.NewGuid();
        var datasetId = Guid.NewGuid();

        var list = new List<SavedAnalysis>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DatasetId = datasetId,
                Title = "Saved 1",
                Notes = "Note 1",
                Dataset = new Dataset { Name = "Sales Dataset" },
                CreatedDate = DateTime.UtcNow
            }
        };

        mockSavedRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var handler = new GetUserSavedAnalysesQueryHandler(mockSavedRepo.Object);

        // Act
        var result = await handler.Handle(new GetUserSavedAnalysesQuery(userId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Saved 1", result[0].Title);
        Assert.Equal("Sales Dataset", result[0].DatasetName);
    }
}
