namespace NexoraBackend.Application.DTOs.Responses;

public class ProductResponseDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<string>? ImageUrls { get; set; }
    public Guid CreatedBy { get; set; } = default!;
}