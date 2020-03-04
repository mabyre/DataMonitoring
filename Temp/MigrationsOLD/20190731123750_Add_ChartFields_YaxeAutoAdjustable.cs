using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class Add_ChartFields_YaxeAutoAdjustable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AxeYIsAutoAdjustableAccordingMinValue",
                table: "IndicatorWidgets",
                nullable: false,
                defaultValueSql:"0");

            migrationBuilder.AddColumn<int>(
                name: "AxeYOffsetFromMinValue",
                table: "IndicatorWidgets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AxeYIsAutoAdjustableAccordingMinValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "AxeYOffsetFromMinValue",
                table: "IndicatorWidgets");
        }
    }
}
