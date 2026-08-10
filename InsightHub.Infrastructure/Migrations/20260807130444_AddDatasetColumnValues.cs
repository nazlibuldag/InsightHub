using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasetColumnValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatasetColumnValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatasetColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetColumnValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetColumnValues_DatasetColumns_DatasetColumnId",
                        column: x => x.DatasetColumnId,
                        principalTable: "DatasetColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatasetColumnValues_DatasetColumnId",
                table: "DatasetColumnValues",
                column: "DatasetColumnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatasetColumnValues");
        }
    }
}
