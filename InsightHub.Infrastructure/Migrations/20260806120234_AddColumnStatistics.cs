using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsightHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageValue",
                table: "DatasetColumns",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxValue",
                table: "DatasetColumns",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MedianValue",
                table: "DatasetColumns",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinValue",
                table: "DatasetColumns",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "StandardDeviation",
                table: "DatasetColumns",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageValue",
                table: "DatasetColumns");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "DatasetColumns");

            migrationBuilder.DropColumn(
                name: "MedianValue",
                table: "DatasetColumns");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "DatasetColumns");

            migrationBuilder.DropColumn(
                name: "StandardDeviation",
                table: "DatasetColumns");
        }
    }
}
