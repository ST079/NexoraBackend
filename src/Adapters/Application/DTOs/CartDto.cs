
namespace NexoraBackend.Application.DTOs;

public record CartDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<CartItemDto> Items,
    decimal Total,
    string Currency,
    int ItemCount

    );