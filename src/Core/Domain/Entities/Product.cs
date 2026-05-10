
using NexoraBackend.Core.Domain.Exceptions;

namespace NexoraBackend.Core.Domain.Entities;

public class Product
{
    public Guid ProductId { get; private set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<string>? ImageUrls { get; set; } = new List<string>();

    public Product()
    {
        ProductId = Guid.NewGuid();
    }

    public void UpdateStock(int stock)
    {
        if (stock < 0)
        {
            throw new DomainException("Stock cannot be negative.");
        }
        Stock = stock;
    }

    public void AddImageUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new DomainException("Url is Empty, Cannot be Added.");
        }

        ImageUrls?.Add(url);
    }
}