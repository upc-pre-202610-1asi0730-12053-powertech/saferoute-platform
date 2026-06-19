using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Powertech.Platform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateTripContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    route_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    driver_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    state = table.Column<string>(type: "longtext", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    end_time = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_trips", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "attendances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    child_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    boarding_state = table.Column<string>(type: "longtext", nullable: false),
                    boarded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    trip_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_attendances", x => x.id);
                    table.ForeignKey(
                        name: "f_k_attendances_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "incidents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    reported_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    trip_id = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_incidents", x => x.id);
                    table.ForeignKey(
                        name: "f_k_incidents_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_attendances_trip_id",
                table: "attendances",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_trip_id",
                table: "incidents",
                column: "trip_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendances");

            migrationBuilder.DropTable(
                name: "incidents");

            migrationBuilder.DropTable(
                name: "trips");
        }
    }
}
