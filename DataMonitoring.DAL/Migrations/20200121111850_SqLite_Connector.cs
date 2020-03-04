using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class SqLite_Connector : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SqlServerConnector_DatabaseName",
                table: "Connectors",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SqlServerConnector_HostName",
                table: "Connectors",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SqlServerConnector_Password",
                table: "Connectors",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SqlServerConnector_UseIntegratedSecurity",
                table: "Connectors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SqlServerConnector_UserName",
                table: "Connectors",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SqlServerConnector_DatabaseName",
                table: "Connectors");

            migrationBuilder.DropColumn(
                name: "SqlServerConnector_HostName",
                table: "Connectors");

            migrationBuilder.DropColumn(
                name: "SqlServerConnector_Password",
                table: "Connectors");

            migrationBuilder.DropColumn(
                name: "SqlServerConnector_UseIntegratedSecurity",
                table: "Connectors");

            migrationBuilder.DropColumn(
                name: "SqlServerConnector_UserName",
                table: "Connectors");
        }
    }
}
