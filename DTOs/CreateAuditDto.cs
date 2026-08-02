namespace AuditModule.DTOs;

public class CreateAuditDto
{
    public Guid EmployeeId { get; set; }

    public string Location { get; set; } = string.Empty;
}
