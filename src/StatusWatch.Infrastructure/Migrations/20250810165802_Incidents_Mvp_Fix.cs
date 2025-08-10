using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StatusWatch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Incidents_Mvp_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    OpenReason = table.Column<string>(type: "text", nullable: true),
                    ResolveReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProbeStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProbeId = table.Column<int>(type: "integer", nullable: false),
                    FailStreak = table.Column<int>(type: "integer", nullable: false),
                    SuccessStreak = table.Column<int>(type: "integer", nullable: false),
                    LastIsSuccess = table.Column<bool>(type: "boolean", nullable: true),
                    LastAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbeStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProbeStatuses_Probes_ProbeId",
                        column: x => x.ProbeId,
                        principalTable: "Probes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IncidentId = table.Column<int>(type: "integer", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentEvents_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentEvents_IncidentId_TimestampUtc",
                table: "IncidentEvents",
                columns: new[] { "IncidentId", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ServiceId_Status",
                table: "Incidents",
                columns: new[] { "ServiceId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProbeStatuses_ProbeId",
                table: "ProbeStatuses",
                column: "ProbeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentEvents");

            migrationBuilder.DropTable(
                name: "ProbeStatuses");

            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}
