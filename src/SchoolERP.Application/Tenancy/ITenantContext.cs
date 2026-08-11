namespace SchoolERP.Application.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
}
