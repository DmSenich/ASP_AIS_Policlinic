using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_AIS_Policlinic.Migrations
{
    public partial class ForConnectOneToMult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diseases_Visitings_VisitingId",
                table: "Diseases");

            migrationBuilder.AlterColumn<int>(
                name: "VisitingId",
                table: "Diseases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Diseases_Visitings_VisitingId",
                table: "Diseases",
                column: "VisitingId",
                principalTable: "Visitings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diseases_Visitings_VisitingId",
                table: "Diseases");

            migrationBuilder.AlterColumn<int>(
                name: "VisitingId",
                table: "Diseases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Diseases_Visitings_VisitingId",
                table: "Diseases",
                column: "VisitingId",
                principalTable: "Visitings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
