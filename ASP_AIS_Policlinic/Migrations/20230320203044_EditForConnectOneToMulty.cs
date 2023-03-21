using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_AIS_Policlinic.Migrations
{
    public partial class EditForConnectOneToMulty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diseases_DiseaseTypes_DiseaseTypeId",
                table: "Diseases");

            migrationBuilder.AlterColumn<int>(
                name: "DiseaseTypeId",
                table: "Diseases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Diseases_DiseaseTypes_DiseaseTypeId",
                table: "Diseases",
                column: "DiseaseTypeId",
                principalTable: "DiseaseTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diseases_DiseaseTypes_DiseaseTypeId",
                table: "Diseases");

            migrationBuilder.AlterColumn<int>(
                name: "DiseaseTypeId",
                table: "Diseases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Diseases_DiseaseTypes_DiseaseTypeId",
                table: "Diseases",
                column: "DiseaseTypeId",
                principalTable: "DiseaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
