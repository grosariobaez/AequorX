namespace SchoolERP.Domain;

internal static class DomainGuard
{
    public static string Required(string value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
        }

        return normalized;
    }

    public static void SameTenant(Guid expectedTenantId, Guid actualTenantId)
    {
        if (expectedTenantId != actualTenantId)
        {
            throw new InvalidOperationException("Cross-tenant relationships are invalid.");
        }
    }
}
