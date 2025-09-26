using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Healthcare_Appointment_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPKIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "ClinicId", "ClinicAddress", "ClinicEmail", "ContactNumber", "EndOperatingHours", "Name", "StartOperatingHours" },
                values: new object[,]
                {
                    { 1, "123 Main St", "info@cityhealth.com", "555-1000", new TimeSpan(0, 17, 0, 0, 0), "City Health Clinic", new TimeSpan(0, 8, 0, 0, 0) },
                    { 2, "456 Elm Ave", "contact@downtownmed.com", "555-2000", new TimeSpan(0, 18, 0, 0, 0), "Downtown Medical Center", new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, "789 Oak Blvd", "hello@suburbanclinic.com", "555-3000", new TimeSpan(0, 16, 0, 0, 0), "Suburban Family Clinic", new TimeSpan(0, 7, 0, 0, 0) },
                    { 4, "101 Pine Rd", "info@specialistcare.com", "555-4000", new TimeSpan(0, 19, 0, 0, 0), "Specialist Care Center", new TimeSpan(0, 10, 0, 0, 0) },
                    { 5, "202 Maple St", "contact@wellnessclinic.com", "555-5000", new TimeSpan(0, 17, 30, 0, 0), "Health & Wellness Clinic", new TimeSpan(0, 8, 30, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "DoctorId", "AppointmentDurationMinutes", "AvailableEnd", "AvailableStart", "DoctorSpecialty", "Email", "FirstName", "LastName", "LicenseNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 30, new TimeSpan(0, 17, 0, 0, 0), new TimeSpan(0, 9, 0, 0, 0), 2, "johndoe@example.com", "John", "Doe", "LIC001", "555-1111" },
                    { 2, 30, new TimeSpan(0, 18, 0, 0, 0), new TimeSpan(0, 10, 0, 0, 0), 3, "janesmith@example.com", "Jane", "Smith", "LIC002", "555-2222" },
                    { 3, 30, new TimeSpan(0, 16, 0, 0, 0), new TimeSpan(0, 8, 0, 0, 0), 1, "alicebrown@example.com", "Alice", "Brown", "LIC003", "555-3333" },
                    { 4, 45, new TimeSpan(0, 17, 0, 0, 0), new TimeSpan(0, 9, 0, 0, 0), 4, "michaelgreen@example.com", "Michael", "Green", "LIC004", "555-4444" },
                    { 5, 30, new TimeSpan(0, 15, 30, 0, 0), new TimeSpan(0, 7, 30, 0, 0), 6, "laurawhite@example.com", "Laura", "White", "LIC005", "555-5555" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientId", "Address", "DateOfBirth", "EmergencyContact", "FirstName", "Gender", "LastName", "PatientEmail", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "12 River Rd", new DateTime(1990, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "555-9000", "Tom", 0, "Anderson", "tomanderson@example.com", "555-6001" },
                    { 2, "34 Lake St", new DateTime(1985, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "555-9001", "Emily", 1, "Clark", "emilyclarke@example.com", "555-6002" },
                    { 3, "56 Hill Ave", new DateTime(2000, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "555-9002", "David", 0, "Lee", "davidlee@example.com", "555-6003" },
                    { 4, "78 Pine St", new DateTime(1995, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "555-9003", "Sophia", 1, "Taylor", "sophiataylor@example.com", "555-6004" },
                    { 5, "90 Oak Rd", new DateTime(1988, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "555-9004", "James", 0, "Wilson", "jameswilson@example.com", "555-6005" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentStatus", "ClinicId", "DoctorId", "Duration", "Note", "PatientId", "Reason", "StartTime" },
                values: new object[,]
                {
                    { 1, 0, 1, 1, 30, "Routine checkup", 1, 0, new DateTime(2025, 9, 25, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 0, 2, 2, 45, "Skin rash consultation", 2, 2, new DateTime(2025, 9, 25, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 0, 3, 3, 30, "Pediatric follow-up", 3, 1, new DateTime(2025, 9, 26, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 0, 4, 4, 45, "Neurology consultation", 4, 9, new DateTime(2025, 9, 26, 14, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 0, 5, 5, 30, "Gynecology checkup", 5, 0, new DateTime(2025, 9, 27, 8, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 0, 1, 1, 30, "Cardio follow-up", 2, 1, new DateTime(2025, 9, 27, 10, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "DoctorClinics",
                columns: new[] { "ClinicId", "DoctorId", "DoctorShift", "EndDate", "StartDate" },
                values: new object[,]
                {
                    { 1, 1, 0, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, 1, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, 0, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 4, 1, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 5, 0, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DoctorClinics",
                keyColumns: new[] { "ClinicId", "DoctorId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "DoctorClinics",
                keyColumns: new[] { "ClinicId", "DoctorId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "DoctorClinics",
                keyColumns: new[] { "ClinicId", "DoctorId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "DoctorClinics",
                keyColumns: new[] { "ClinicId", "DoctorId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "DoctorClinics",
                keyColumns: new[] { "ClinicId", "DoctorId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "ClinicId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "DoctorId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: 5);
        }
    }
}
