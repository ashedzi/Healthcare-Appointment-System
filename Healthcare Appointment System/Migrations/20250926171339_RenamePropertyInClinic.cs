using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Healthcare_Appointment_System.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertyInClinic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Clinics",
                newName: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Clinics",
                newName: "ContactNumber");
        }
    }
}
