using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class ColorHtml : Migration
    {
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.RenameTable( name: "ColorClasses", newName: "ColorHtml", schema: "dbo" );
        }

        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.RenameTable( name: "ColorHtml", newName: "ColorClasses", schema: "dbo" );
        }
    }
}
