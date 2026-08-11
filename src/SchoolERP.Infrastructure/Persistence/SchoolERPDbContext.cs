using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Tenancy;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Attendance;
using SchoolERP.Domain.Grading;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Infrastructure.Persistence;

public sealed class SchoolERPDbContext(
    DbContextOptions<SchoolERPDbContext> options,
    ITenantContext tenantContext)
    : DbContext(options)
{
    private Guid CurrentTenantId => tenantContext.TenantId;

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Campus> Campuses => Set<Campus>();

    public DbSet<Person> People => Set<Person>();

    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();

    public DbSet<StudentRelationship> StudentRelationships => Set<StudentRelationship>();

    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();

    public DbSet<GradeLevel> GradeLevels => Set<GradeLevel>();

    public DbSet<Section> Sections => Set<Section>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<GradeCorrection> GradeCorrections => Set<GradeCorrection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolERPDbContext).Assembly);

        modelBuilder.Entity<Tenant>().HasQueryFilter(entity => entity.Id == CurrentTenantId);
        modelBuilder.Entity<Campus>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Person>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<StudentProfile>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<StudentRelationship>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<AcademicYear>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<GradeLevel>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Section>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Enrollment>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<AttendanceRecord>().HasQueryFilter(
            entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Assessment>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Grade>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<GradeCorrection>().HasQueryFilter(entity => entity.TenantId == CurrentTenantId);
    }
}
