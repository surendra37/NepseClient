using Microsoft.EntityFrameworkCore.Migrations;

namespace NepseApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "live",
                columns: table => new
                {
                    symbol = table.Column<string>(type: "TEXT", nullable: false),
                    ltp = table.Column<double>(type: "REAL", nullable: false),
                    ltv = table.Column<double>(type: "REAL", nullable: false),
                    point_change = table.Column<double>(type: "REAL", nullable: false),
                    percent_change = table.Column<double>(type: "REAL", nullable: false),
                    open = table.Column<double>(type: "REAL", nullable: false),
                    high = table.Column<double>(type: "REAL", nullable: false),
                    low = table.Column<double>(type: "REAL", nullable: false),
                    close = table.Column<double>(type: "REAL", nullable: false),
                    volume = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_live", x => x.symbol);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "live");
        }
    }
}
