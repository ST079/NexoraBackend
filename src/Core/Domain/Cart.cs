using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

public class Cart : Auditable
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public Money Total => _items.Aggregate(
        Money.Zero("USD"),
        (acc, item) => acc.Add(item.LineTotal));

    public int ItemCount => _items.Sum(i => i.Quantity);

    private Cart() { }

    public static Cart Create(Guid userId) => new() { UserId = userId };

    public void AddItem(Product product, int quantity)
    {
        if (!product.IsActive)
            throw new DomainException("Product is not available.");

        var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null)
            existing.UpdateQuantity(existing.Quantity + quantity);
        else
            _items.Add(CartItem.Create(Id, product.Id, product.Name, quantity, product.Price));
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new DomainException("Item not in cart.");
        _items.Remove(item);
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (quantity <= 0)
        {
            RemoveItem(productId);
            return;
        }
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new DomainException("Item not in cart.");
        item.UpdateQuantity(quantity);
    }

    public void Clear() => _items.Clear();
}