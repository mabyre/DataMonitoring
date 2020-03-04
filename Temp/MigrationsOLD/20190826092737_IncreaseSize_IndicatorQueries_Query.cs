using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class IncreaseSize_IndicatorQueries_Query : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Query",
                table: "IndicatorQueries",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 3000,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Query",
                table: "IndicatorQueries",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
