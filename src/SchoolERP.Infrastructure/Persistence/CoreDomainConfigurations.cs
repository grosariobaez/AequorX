using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Infrastructure.Persistence;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => entity.Code).IsUnique();
    }
}

internal sealed class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("Campuses");
        builder.HasKey(entity => entity.Id);
        builder.HasAlternateKey(entity => new { entity.TenantId, entity.Id });
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => new { entity.TenantId, entity.Code }).IsUnique();
        builder
            .HasOne(entity => entity.Tenant)
            .WithMany()
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");
        builder.HasKey(entity => entity.Id);
        builder.HasAlternateKey(entity => new { entity.TenantId, entity.Id });
        builder.Property(entity => entity.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.MiddleName).HasMaxLength(100);
        builder.Property(entity => entity.LastName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.SecondLastName).HasMaxLength(100);
        builder.Property(entity => entity.PreferredName).HasMaxLength(100);
        builder.Property(entity => entity.Email).HasMaxLength(254);
        builder.Property(entity => entity.Phone).HasMaxLength(50);
        builder.HasIndex(entity => entity.TenantId);
        builder
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("StudentProfiles");
        builder.HasKey(entity => entity.PersonId);
        builder.HasAlternateKey(entity => new { entity.TenantId, entity.PersonId });
        builder.Property(entity => entity.StudentNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => new { entity.TenantId, entity.StudentNumber }).IsUnique();
        builder
            .HasOne(entity => entity.Person)
            .WithOne()
            .HasForeignKey<StudentProfile>(entity => new { entity.TenantId, entity.PersonId })
            .HasPrincipalKey<Person>(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class StudentRelationshipConfiguration
    : IEntityTypeConfiguration<StudentRelationship>
{
    public void Configure(EntityTypeBuilder<StudentRelationship> builder)
    {
        builder.ToTable("StudentRelationships");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.RelationshipType).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(entity => new
        {
            entity.TenantId,
            entity.StudentPersonId,
            entity.RelatedPersonId,
            entity.RelationshipType
        }).IsUnique();
        builder
            .HasOne(entity => entity.Student)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.StudentPersonId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(entity => entity.RelatedPerson)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.RelatedPersonId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.ToTable("AcademicYears");
        builder.HasKey(entity => entity.Id);
        builder.HasAlternateKey(entity => new { entity.TenantId, entity.Id });
        builder.Property(entity => entity.Name).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(20);
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_AcademicYears_DateRange",
            "[StartDate] < [EndDate]"));
        builder.HasIndex(entity => entity.TenantId);
        builder
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class GradeLevelConfiguration : IEntityTypeConfiguration<GradeLevel>
{
    public void Configure(EntityTypeBuilder<GradeLevel> builder)
    {
        builder.ToTable("GradeLevels");
        builder.HasKey(entity => entity.Id);
        builder.HasAlternateKey(entity => new { entity.TenantId, entity.Id });
        builder.Property(entity => entity.Name).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => new { entity.TenantId, entity.Code }).IsUnique();
        builder
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");
        builder.HasKey(entity => entity.Id);
        builder.HasAlternateKey(entity => new
        {
            entity.TenantId,
            entity.AcademicYearId,
            entity.Id
        });
        builder.Property(entity => entity.Name).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => new
        {
            entity.TenantId,
            entity.AcademicYearId,
            entity.Code
        }).IsUnique();
        builder
            .HasOne(entity => entity.AcademicYear)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.AcademicYearId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(entity => entity.GradeLevel)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.GradeLevelId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(entity => entity.Campus)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.CampusId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(entity => new
        {
            entity.TenantId,
            entity.StudentPersonId,
            entity.AcademicYearId
        });
        builder
            .HasOne(entity => entity.Student)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.StudentPersonId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.PersonId })
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(entity => entity.AcademicYear)
            .WithMany()
            .HasForeignKey(entity => new { entity.TenantId, entity.AcademicYearId })
            .HasPrincipalKey(entity => new { entity.TenantId, entity.Id })
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(entity => entity.Section)
            .WithMany()
            .HasForeignKey(entity => new
            {
                entity.TenantId,
                entity.AcademicYearId,
                entity.SectionId
            })
            .HasPrincipalKey(entity => new
            {
                entity.TenantId,
                entity.AcademicYearId,
                entity.Id
            })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
