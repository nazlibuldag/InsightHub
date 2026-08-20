using System;
using InsightHub.Domain.Entities;
using InsightHub.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace InsightHub.Tests;

public class AuthTests
{
    [Fact]
    public void PasswordHasher_ShouldHashAndVerifyPasswordCorrectly()
    {
        // Arrange
        var hasher = new PasswordHasher();
        var rawPassword = "SuperSecretPassword123!";

        // Act
        var hash = hasher.HashPassword(rawPassword);
        var isMatch = hasher.VerifyPassword(rawPassword, hash);
        var isInvalidMatch = hasher.VerifyPassword("WrongPassword", hash);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEqual(rawPassword, hash);
        Assert.True(isMatch);
        Assert.False(isInvalidMatch);
    }

    [Fact]
    public void JwtTokenGenerator_ShouldGenerateValidTokens()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JwtSettings:Secret"]).Returns("InsightHub_Super_Secret_Key_For_Jwt_Token_Generation_2026!");
        mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("InsightHubAPI");
        mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("InsightHubWeb");
        mockConfig.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("60");

        var tokenGenerator = new JwtTokenGenerator(mockConfig.Object);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@insighthub.com",
            PasswordHash = "hashed"
        };

        // Act
        var token = tokenGenerator.GenerateToken(user);
        var refreshToken = tokenGenerator.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
    }
}
