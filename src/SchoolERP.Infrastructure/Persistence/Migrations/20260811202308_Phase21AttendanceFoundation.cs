using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase21AttendanceFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Sections_TenantId_Id",
                table: "Sections",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Enrollments_TenantId_Id_SectionId",
                table: "Enrollments",
                columns: new[] { "TenantId", "Id", "SectionId" });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Enrollments_TenantId_EnrollmentId_SectionId",
                        columns: x => new { x.TenantId, x.EnrollmentId, x.SectionId },
                        principalTable: "Enrollments",
                        principalColumns: new[] { "TenantId", "Id", "SectionId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Sections_TenantId_SectionId",
                        columns: x => new { x.TenantId, x.SectionId },
                        principalTable: "Sections",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_TenantId_EnrollmentId_AttendanceDate",
                table: "AttendanceRecords",
                columns: new[] { "TenantId", "EnrollmentId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_TenantId_EnrollmentId_SectionId",
                table: "AttendanceRecords",
                columns: new[] { "TenantId", "EnrollmentId", "SectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_TenantId_SectionId",
                table: "AttendanceRecords",
                columns: new[] { "TenantId", "SectionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Sections_TenantId_Id",
                table: "Sections");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Enrollments_TenantId_Id_SectionId",
                table: "Enrollments");
        }
    }
}
