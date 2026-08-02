namespace AuditModule.Models;

public class Audit
{
    public Guid AuditId { get; set; }

    public Guid EmployeeId { get; set; }

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime TimeOut { get; set; }

    public DateTime? LogoutTime { get; set; }

    public DateTime LastActivityAt { get; set; }
}
