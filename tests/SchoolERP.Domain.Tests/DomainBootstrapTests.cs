using System.Reflection;

namespace SchoolERP.Domain.Tests;

public sealed class DomainBootstrapTests
{
    [Fact]
    public void Domain_contains_no_premature_public_types()
    {
        var domain = Assembly.Load("SchoolERP.Domain");

        Assert.Empty(domain.ExportedTypes);
    }
}
