using Microsoft.EntityFrameworkCore;

namespace SchoolERP.Infrastructure.Persistence;

public sealed class SchoolERPDbContext(DbContextOptions<SchoolERPDbContext> options)
    : DbContext(options);
