using System.Net;

namespace SchoolERP.IntegrationTests;

public sealed class HealthEndpointTests
{
    private const string UnavailableSql =
        "Server=127.0.0.1,1;Database=Unavailable;User Id=unavailable;Password=unavailable;" +
        "TrustServerCertificate=True;Connect Timeout=1";

    [Fact]
    public async Task Liveness_is_healthy_when_database_is_unavailable()
    {
        await using var factory = new ApiFactory(UnavailableSql);
        using var client = factory.CreateHttpsClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Readiness_is_unhealthy_when_database_is_unavailable()
    {
        await using var factory = new ApiFactory(UnavailableSql);
        using var client = factory.CreateHttpsClient();

        var response = await client.GetAsync("/health/ready");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.DoesNotContain("Password", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Server=", body, StringComparison.OrdinalIgnoreCase);
    }
}
