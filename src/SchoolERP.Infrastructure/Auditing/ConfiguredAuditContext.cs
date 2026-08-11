using Microsoft.Extensions.Configuration;
using SchoolERP.Application.Auditing;

namespace SchoolERP.Infrastructure.Auditing;

internal sealed class ConfiguredAuditContext : IAuditContext
{
    public ConfiguredAuditContext(IConfiguration configuration)
    {
        var actor = configuration["Audit:Actor"];
        if (string.IsNullOrWhiteSpace(actor))
        {
            throw new InvalidOperationException(
                "A server-side Audit:Actor configuration value is required.");
        }

        Actor = actor.Trim();
    }

    public string Actor { get; }
}
