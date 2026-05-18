using NexoraBackend.Application.DTOs;

namespace NexoraBackend.API.GraphQL.Types;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Description("A registered user account.");
        descriptor.Field(u => u.Id).Description("Unique user identifier.");
        // Never expose password hash — no field for it exists on UserDto
        descriptor.Field(u => u.Email).Description("Verified email address.");
        descriptor.Field(u => u.Role).Description("Access role: Customer, Admin, or SuperAdmin.");
    }
}