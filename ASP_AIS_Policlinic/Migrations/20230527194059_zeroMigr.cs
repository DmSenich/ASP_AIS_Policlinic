using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_AIS_Policlinic.Migrations
{
    public partial class zeroMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialty_Doctors_DoctorsId",
                table: "DoctorSpecialty");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialty_Specialties_SpecialtiesId",
                table: "DoctorSpecialty");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitings_Doctors_DoctorId",
                table: "Visitings");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitings_Patients_PatientId",
                table: "Visitings");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialty_Doctors_DoctorsId",
                table: "DoctorSpecialty",
                column: "DoctorsId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialty_Specialties_SpecialtiesId",
                table: "DoctorSpecialty",
                column: "SpecialtiesId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitings_Doctors_DoctorId",
                table: "Visitings",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Visitings_Patients_PatientId",
                table: "Visitings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialty_Doctors_DoctorsId",
                table: "DoctorSpecialty");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialty_Specialties_SpecialtiesId",
                table: "DoctorSpecialty");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitings_Doctors_DoctorId",
                table: "Visitings");

            migrationBuilder.DropForeignKey(
                name: "FK_Visitings_Patients_PatientId",
                table: "Visitings");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialty_Doctors_DoctorsId",
                table: "DoctorSpecialty",
                column: "DoctorsId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialty_Specialties_SpecialtiesId",
                table: "DoctorSpecialty",
                column: "SpecialtiesId",
                principalTable: "Specialties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitings_Doctors_DoctorId",
                table: "Visitings",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitings_Patients_PatientId",
                table: "Visitings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }
    }
}
