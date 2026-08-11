namespace SchoolERP.Application.Auditing;

public interface IAuditContext
{
    string Actor { get; }
}
