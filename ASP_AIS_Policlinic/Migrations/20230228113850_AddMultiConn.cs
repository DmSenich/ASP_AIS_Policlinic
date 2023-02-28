using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_AIS_Policlinic.Migrations
{
    public partial class AddMultiConn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Apartment",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VisitingId",
                table: "Diseases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoctorSpecialty",
                columns: table => new
                {
                    DoctorsId = table.Column<int>(type: "int", nullable: false),
                    SpecialtiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSpecialty", x => new { x.DoctorsId, x.SpecialtiesId });
                    table.ForeignKey(
                        name: "FK_DoctorSpecialty_Doctors_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorSpecialty_Specialties_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diseases_VisitingId",
                table: "Diseases",
                column: "VisitingId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecialty_SpecialtiesId",
                table: "DoctorSpecialty",
                column: "SpecialtiesId");

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

            migrationBuilder.DropTable(
                name: "DoctorSpecialty");

            migrationBuilder.DropIndex(
                name: "IX_Diseases_VisitingId",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "VisitingId",
                table: "Diseases");

            migrationBuilder.AlterColumn<int>(
                name: "Apartment",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
