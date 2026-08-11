using System.Reflection;

namespace SchoolERP.Application.Tests;

public sealed class ApplicationBootstrapTests
{
    [Fact]
    public void Application_contains_no_speculative_public_types()
    {
        var application = Assembly.Load("SchoolERP.Application");

        Assert.Empty(application.ExportedTypes);
    }
}
