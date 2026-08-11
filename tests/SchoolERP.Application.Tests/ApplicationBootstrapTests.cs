using SchoolERP.Application.Auditing;
using SchoolERP.Application.Tenancy;

namespace SchoolERP.Application.Tests;

public sealed class ApplicationBootstrapTests
{
    [Fact]
    public void Application_exposes_only_current_request_context_contracts()
    {
        var exportedTypes = typeof(ITenantContext).Assembly.ExportedTypes
            .OrderBy(type => type.FullName)
            .ToArray();

        Assert.Equal(
            new[] { typeof(IAuditContext), typeof(ITenantContext) }
                .OrderBy(type => type.FullName),
            exportedTypes);
    }
}
