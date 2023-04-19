using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_AIS_Policlinic.Migrations.Identity
{
    public partial class ModelIdForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "AspNetUsers");
        }
    }
}
