
using NexoraBackend.Core.Events;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

public class Product : Auditable
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public Money Price { get; private set; }

    public Money? CompareAtPrice { get; private set; }
    /*
    discount UI
    "Was $100 → Now $80"
    */


    public int StockQuantity { get; private set; }
    public int LowStockThreshold { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsFeatured { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private readonly List<string> _imageUrls = [];
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly(); //readable outside, writable only inside

    private readonly List<Review> _reviews = [];
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Product() { } // for EF Core

    public static Product Create(string name, string description, decimal price, string currency, int stockQuantity, Guid categoryId)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Slug = Slug.Create(name),
            Price = new Money(price, currency),
            StockQuantity = stockQuantity,
            CategoryId = categoryId
        };
        return product;
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        var previousStock = StockQuantity;
        StockQuantity = quantity;

        if (StockQuantity <= LowStockThreshold && previousStock > LowStockThreshold)
        {
            // Raise low stock event
            AddDomainEvent(new LowStockEvent(Id, Name, StockQuantity));
        }
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to reserve must be greater than zero.");
        if (StockQuantity < quantity)
            throw new InsufficientStockException(Id, Name, quantity, StockQuantity);

        StockQuantity -= quantity;

        if (StockQuantity <= LowStockThreshold)
        {
            // Raise low stock event
            AddDomainEvent(new LowStockEvent(Id, Name, StockQuantity));
        }
    }

    public void ReleaseStock(int quantity) => StockQuantity += quantity;

    public void UpdatePrice(decimal amount, string currency) => Price = new Money(amount, currency);

    public void AddImage(string imageUrl) => _imageUrls.Add(imageUrl);

    public decimal AverageRating => _reviews.Count == 0 ? 0 : (decimal)_reviews.Average(r => r.Rating);

}