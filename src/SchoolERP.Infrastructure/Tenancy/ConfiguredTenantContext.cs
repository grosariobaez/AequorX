using Microsoft.Extensions.Configuration;
using SchoolERP.Application.Tenancy;

namespace SchoolERP.Infrastructure.Tenancy;

internal sealed class ConfiguredTenantContext : ITenantContext
{
    public ConfiguredTenantContext(IConfiguration configuration)
    {
        var configuredTenantId = configuration["Tenant:Id"];
        if (!Guid.TryParse(configuredTenantId, out var tenantId) || tenantId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A valid server-side Tenant:Id configuration value is required.");
        }

        TenantId = tenantId;
    }

    public Guid TenantId { get; }
}
