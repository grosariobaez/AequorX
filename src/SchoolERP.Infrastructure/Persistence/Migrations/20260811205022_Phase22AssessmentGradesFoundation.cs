using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase22AssessmentGradesFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Enrollments_TenantId_Id",
                table: "Enrollments",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssessmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MaximumScore = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.UniqueConstraint("AK_Assessments_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Assessments_Sections_TenantId_SectionId",
                        columns: x => new { x.TenantId, x.SectionId },
                        principalTable: "Sections",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.UniqueConstraint("AK_Grades_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Grades_Assessments_TenantId_AssessmentId",
                        columns: x => new { x.TenantId, x.AssessmentId },
                        principalTable: "Assessments",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Enrollments_TenantId_EnrollmentId",
                        columns: x => new { x.TenantId, x.EnrollmentId },
                        principalTable: "Enrollments",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradeCorrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousScore = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NewScore = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CorrectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CorrectedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeCorrections_Grades_TenantId_GradeId",
                        columns: x => new { x.TenantId, x.GradeId },
                        principalTable: "Grades",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_TenantId_SectionId_AssessmentDate",
                table: "Assessments",
                columns: new[] { "TenantId", "SectionId", "AssessmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GradeCorrections_TenantId_GradeId_CorrectedAt",
                table: "GradeCorrections",
                columns: new[] { "TenantId", "GradeId", "CorrectedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TenantId_AssessmentId_EnrollmentId",
                table: "Grades",
                columns: new[] { "TenantId", "AssessmentId", "EnrollmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TenantId_EnrollmentId",
                table: "Grades",
                columns: new[] { "TenantId", "EnrollmentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GradeCorrections");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Enrollments_TenantId_Id",
                table: "Enrollments");
        }
    }
}
