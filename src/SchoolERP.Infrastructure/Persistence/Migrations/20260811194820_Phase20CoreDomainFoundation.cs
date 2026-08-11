using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase20CoreDomainFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                    table.UniqueConstraint("AK_AcademicYears_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.CheckConstraint("CK_AcademicYears_DateRange", "[StartDate] < [EndDate]");
                    table.ForeignKey(
                        name: "FK_AcademicYears_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                    table.UniqueConstraint("AK_Campuses_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Campuses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradeLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeLevels", x => x.Id);
                    table.UniqueConstraint("AK_GradeLevels_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_GradeLevels_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecondLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreferredName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.UniqueConstraint("AK_People_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_People_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.UniqueConstraint("AK_Sections_TenantId_AcademicYearId_Id", x => new { x.TenantId, x.AcademicYearId, x.Id });
                    table.ForeignKey(
                        name: "FK_Sections_AcademicYears_TenantId_AcademicYearId",
                        columns: x => new { x.TenantId, x.AcademicYearId },
                        principalTable: "AcademicYears",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sections_Campuses_TenantId_CampusId",
                        columns: x => new { x.TenantId, x.CampusId },
                        principalTable: "Campuses",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sections_GradeLevels_TenantId_GradeLevelId",
                        columns: x => new { x.TenantId, x.GradeLevelId },
                        principalTable: "GradeLevels",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.PersonId);
                    table.UniqueConstraint("AK_StudentProfiles_TenantId_PersonId", x => new { x.TenantId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_StudentProfiles_People_TenantId_PersonId",
                        columns: x => new { x.TenantId, x.PersonId },
                        principalTable: "People",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsPrimaryContact = table.Column<bool>(type: "bit", nullable: false),
                    IsAuthorizedPickup = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRelationships_People_TenantId_RelatedPersonId",
                        columns: x => new { x.TenantId, x.RelatedPersonId },
                        principalTable: "People",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentRelationships_People_TenantId_StudentPersonId",
                        columns: x => new { x.TenantId, x.StudentPersonId },
                        principalTable: "People",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnrollmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_AcademicYears_TenantId_AcademicYearId",
                        columns: x => new { x.TenantId, x.AcademicYearId },
                        principalTable: "AcademicYears",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Sections_TenantId_AcademicYearId_SectionId",
                        columns: x => new { x.TenantId, x.AcademicYearId, x.SectionId },
                        principalTable: "Sections",
                        principalColumns: new[] { "TenantId", "AcademicYearId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_StudentProfiles_TenantId_StudentPersonId",
                        columns: x => new { x.TenantId, x.StudentPersonId },
                        principalTable: "StudentProfiles",
                        principalColumns: new[] { "TenantId", "PersonId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_TenantId",
                table: "AcademicYears",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_TenantId_Code",
                table: "Campuses",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TenantId_AcademicYearId_SectionId",
                table: "Enrollments",
                columns: new[] { "TenantId", "AcademicYearId", "SectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TenantId_StudentPersonId_AcademicYearId",
                table: "Enrollments",
                columns: new[] { "TenantId", "StudentPersonId", "AcademicYearId" });

            migrationBuilder.CreateIndex(
                name: "IX_GradeLevels_TenantId_Code",
                table: "GradeLevels",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_TenantId",
                table: "People",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TenantId_AcademicYearId_Code",
                table: "Sections",
                columns: new[] { "TenantId", "AcademicYearId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TenantId_CampusId",
                table: "Sections",
                columns: new[] { "TenantId", "CampusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TenantId_GradeLevelId",
                table: "Sections",
                columns: new[] { "TenantId", "GradeLevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_TenantId_StudentNumber",
                table: "StudentProfiles",
                columns: new[] { "TenantId", "StudentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentRelationships_TenantId_RelatedPersonId",
                table: "StudentRelationships",
                columns: new[] { "TenantId", "RelatedPersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRelationships_TenantId_StudentPersonId_RelatedPersonId_RelationshipType",
                table: "StudentRelationships",
                columns: new[] { "TenantId", "StudentPersonId", "RelatedPersonId", "RelationshipType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Code",
                table: "Tenants",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "StudentRelationships");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Campuses");

            migrationBuilder.DropTable(
                name: "GradeLevels");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
