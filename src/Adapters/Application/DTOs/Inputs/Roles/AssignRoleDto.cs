namespace NexoraBackend.Application.DTOs.Inputs.Roles;

public class AssignRoleDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
}