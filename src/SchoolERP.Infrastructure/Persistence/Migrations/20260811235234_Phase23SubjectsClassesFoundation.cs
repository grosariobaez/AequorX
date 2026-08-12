using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase23SubjectsClassesFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [Assessments])
                    THROW 51000, 'Phase 2.3 requires an explicit Subject/Class mapping for existing Assessments before migration.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Sections_TenantId_SectionId",
                table: "Assessments");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Assessments",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_TenantId_SectionId_AssessmentDate",
                table: "Assessments",
                newName: "IX_Assessments_TenantId_ClassId_AssessmentDate");

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.UniqueConstraint("AK_Subjects_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Subjects_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.UniqueConstraint("AK_Classes_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Classes_Sections_TenantId_SectionId",
                        columns: x => new { x.TenantId, x.SectionId },
                        principalTable: "Sections",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Classes_Subjects_TenantId_SubjectId",
                        columns: x => new { x.TenantId, x.SubjectId },
                        principalTable: "Subjects",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TenantId_SectionId_SubjectId",
                table: "Classes",
                columns: new[] { "TenantId", "SectionId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TenantId_SubjectId",
                table: "Classes",
                columns: new[] { "TenantId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TenantId_Code",
                table: "Subjects",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Classes_TenantId_ClassId",
                table: "Assessments",
                columns: new[] { "TenantId", "ClassId" },
                principalTable: "Classes",
                principalColumns: new[] { "TenantId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM [Assessments])
                    THROW 51001, 'Phase 2.3 cannot be reversed while Assessments reference Classes.', 1;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Classes_TenantId_ClassId",
                table: "Assessments");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Assessments",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_TenantId_ClassId_AssessmentDate",
                table: "Assessments",
                newName: "IX_Assessments_TenantId_SectionId_AssessmentDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Sections_TenantId_SectionId",
                table: "Assessments",
                columns: new[] { "TenantId", "SectionId" },
                principalTable: "Sections",
                principalColumns: new[] { "TenantId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
