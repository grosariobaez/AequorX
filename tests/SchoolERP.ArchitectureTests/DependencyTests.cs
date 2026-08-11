using System.Reflection;
using System.Xml.Linq;

namespace SchoolERP.ArchitectureTests;

public sealed class DependencyTests
{
    private static readonly string[] ForbiddenDomainDependencies =
    [
        "SchoolERP.Infrastructure",
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore"
    ];

    [Fact]
    public void Domain_has_no_forbidden_dependencies()
    {
        var references = Assembly
            .Load("SchoolERP.Domain")
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        foreach (var forbiddenDependency in ForbiddenDomainDependencies)
        {
            Assert.DoesNotContain(
                references,
                reference => reference.StartsWith(forbiddenDependency, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Domain_project_declares_no_forbidden_references()
    {
        var repositoryRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            "SchoolERP.Domain",
            "SchoolERP.Domain.csproj");
        var project = XDocument.Load(projectPath);
        var references = project
            .Descendants()
            .Where(element => element.Name.LocalName is "ProjectReference" or "PackageReference")
            .Select(element => element.Attribute("Include")?.Value ?? string.Empty)
            .ToArray();

        foreach (var forbiddenDependency in ForbiddenDomainDependencies)
        {
            Assert.DoesNotContain(
                references,
                reference => reference.Contains(forbiddenDependency, StringComparison.OrdinalIgnoreCase));
        }
    }
}
