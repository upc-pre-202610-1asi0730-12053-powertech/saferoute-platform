using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Powertech.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS `vehicles` (
                    `id` char(36) NOT NULL,
                    `organization_id` char(36) NOT NULL,
                    `plate` varchar(20) NOT NULL,
                    `model` varchar(80) NOT NULL,
                    `capacity` int NOT NULL,
                    `status` varchar(20) NOT NULL,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET utf8mb4;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `vehicles`;");
        }
    }
}
