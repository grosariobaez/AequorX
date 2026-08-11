using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SchoolERP.IntegrationTests;

internal sealed class ApiFactory(string? connectionString = null) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        var testConnectionString = connectionString
            ?? Environment.GetEnvironmentVariable("SCHOOLERP_TEST_SQL_CONNECTION_STRING")
            ?? "Server=.\\SQLEXPRESS;Database=SchoolERP_IntegrationTests;Trusted_Connection=True;" +
               "TrustServerCertificate=True;MultipleActiveResultSets=True";

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SchoolERP"] = testConnectionString
            });
        });
    }

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });
}
