using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SchoolERP.IntegrationTests;

internal sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _testConnectionString;

    public Guid TenantId { get; } = Guid.NewGuid();

    public ApiFactory(string? connectionString = null, bool isolateDatabase = false)
    {
        var configuredConnectionString = connectionString
            ?? Environment.GetEnvironmentVariable("SCHOOLERP_TEST_SQL_CONNECTION_STRING")
            ?? "Server=.\\SQLEXPRESS;Database=SchoolERP_IntegrationTests;Trusted_Connection=True;" +
               "TrustServerCertificate=True;MultipleActiveResultSets=True";

        if (isolateDatabase)
        {
            var builder = new SqlConnectionStringBuilder(configuredConnectionString)
            {
                InitialCatalog = $"SchoolERP_IntegrationTests_{Guid.NewGuid():N}"
            };

            _testConnectionString = builder.ConnectionString;
        }
        else
        {
            _testConnectionString = configuredConnectionString;
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SchoolERP"] = _testConnectionString,
                ["Tenant:Id"] = TenantId.ToString(),
                ["Audit:Actor"] = "integration-test-user"
            });
        });
    }

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });
}
