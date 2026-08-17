using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndSavedAnalysisEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [Users] ADD [IsActive] bit NOT NULL DEFAULT 1;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[SavedAnalyses]') AND name = 'AnalysisType')
                BEGIN
                    ALTER TABLE [SavedAnalyses] ADD [AnalysisType] nvarchar(max) NOT NULL DEFAULT N'General';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[SavedAnalyses]') AND name = 'ConfigurationJson')
                BEGIN
                    ALTER TABLE [SavedAnalyses] ADD [ConfigurationJson] nvarchar(max) NOT NULL DEFAULT N'{}';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[SavedAnalyses]') AND name = 'ResultJson')
                BEGIN
                    ALTER TABLE [SavedAnalyses] ADD [ResultJson] nvarchar(max) NOT NULL DEFAULT N'{}';
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
                BEGIN
                    CREATE TABLE [AuditLogs] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NULL,
                        [UserEmail] nvarchar(max) NOT NULL,
                        [Action] nvarchar(max) NOT NULL,
                        [EntityName] nvarchar(max) NOT NULL,
                        [EntityId] nvarchar(max) NOT NULL,
                        [IpAddress] nvarchar(max) NOT NULL,
                        [Details] nvarchar(max) NOT NULL,
                        [Timestamp] datetime2 NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Workspaces')
                BEGIN
                    CREATE TABLE [Workspaces] (
                        [Id] uniqueidentifier NOT NULL,
                        [Name] nvarchar(max) NOT NULL,
                        [Description] nvarchar(max) NOT NULL,
                        [OwnerId] uniqueidentifier NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        CONSTRAINT [PK_Workspaces] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Workspaces_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkspaceMembers')
                BEGIN
                    CREATE TABLE [WorkspaceMembers] (
                        [Id] uniqueidentifier NOT NULL,
                        [WorkspaceId] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [Role] nvarchar(max) NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NULL,
                        CONSTRAINT [PK_WorkspaceMembers] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_WorkspaceMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_WorkspaceMembers_Workspaces_WorkspaceId] FOREIGN KEY ([WorkspaceId]) REFERENCES [Workspaces] ([Id]) ON DELETE NO ACTION
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
