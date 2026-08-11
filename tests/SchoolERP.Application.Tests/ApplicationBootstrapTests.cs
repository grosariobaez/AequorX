using SchoolERP.Application.Tenancy;

namespace SchoolERP.Application.Tests;

public sealed class ApplicationBootstrapTests
{
    [Fact]
    public void Application_exposes_only_the_current_tenant_contract()
    {
        var exportedTypes = typeof(ITenantContext).Assembly.ExportedTypes.ToArray();

        Assert.Equal([typeof(ITenantContext)], exportedTypes);
    }
}
