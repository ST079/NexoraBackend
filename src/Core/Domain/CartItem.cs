using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

/// CartItem is a child entity of Cart — it has no independent lifecycle.
/// It stores a snapshot of the product price at the time it was added.
/// Why snapshot? So the cart total doesn't silently change if an admin
/// updates the product price mid-session.
public class CartItem : Base
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public string? ProductImageUrl { get; private set; }
    public int Quantity { get; private set; }

    // Price snapshot — captured when item is added to cart
    public Money UnitPrice { get; private set; } = null!;

    // Computed — not stored
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private CartItem() { }

    public static CartItem Create(Guid cartId, Guid productId, string productName,
        int quantity, Money unitPrice, string? imageUrl = null)
    {
        if (quantity <= 0)
            throw new DomainException("Cart item quantity must be positive.");

        return new CartItem
        {
            CartId = cartId,
            ProductId = productId,
            ProductName = productName,
            ProductImageUrl = imageUrl,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be at least 1.");
        Quantity = newQuantity;
    }

    /// Refreshes the price snapshot — call this if you want to
    /// sync cart prices with current product prices (optional policy).
    public void RefreshPrice(Money currentPrice) =>
        UnitPrice = currentPrice;
}