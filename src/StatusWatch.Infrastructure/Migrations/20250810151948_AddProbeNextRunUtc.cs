using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatusWatch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProbeNextRunUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunAtUtc",
                table: "Probes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Probes_NextRunAtUtc",
                table: "Probes",
                column: "NextRunAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Probes_NextRunAtUtc",
                table: "Probes");

            migrationBuilder.DropColumn(
                name: "NextRunAtUtc",
                table: "Probes");
        }
    }
}
