using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class Add_Gauge_params : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GaugeRange1Color",
                table: "IndicatorWidgets",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange1MaxValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange1MinValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GaugeRange2Color",
                table: "IndicatorWidgets",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GaugeRange2Displayed",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange2MaxValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange2MinValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GaugeRange3Color",
                table: "IndicatorWidgets",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GaugeRange3Displayed",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange3MaxValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GaugeRange3MinValue",
                table: "IndicatorWidgets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GaugeTargetColor",
                table: "IndicatorWidgets",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GaugeRange1Color",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange1MaxValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange1MinValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange2Color",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange2Displayed",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange2MaxValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange2MinValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange3Color",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange3Displayed",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange3MaxValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeRange3MinValue",
                table: "IndicatorWidgets");

            migrationBuilder.DropColumn(
                name: "GaugeTargetColor",
                table: "IndicatorWidgets");
        }
    }
}
