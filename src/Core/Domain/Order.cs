using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Events;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;


namespace NexoraBackend.Core.Domain;

public class Order : Auditable
{

    public string OrderNumber { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Address ShippingAddress { get; private set; } = null!;
    public Money Subtotal { get; private set; } = null!;
    public Money TaxAmount { get; private set; } = null!;
    public Money ShippingCost { get; private set; } = null!;
    public Money DiscountAmount { get; private set; } = null!;
    public Money Total { get; private set; } = null!;
    public string? CouponCode { get; private set; }
    public string? Notes { get; private set; }
    public Payment? Payment { get; private set; }

    private readonly List<OrderItem> _items = [];
    //outside world can read, but NOT modify directly
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // for EF Core


    public static Order Create(Guid userId, Address shippingAddress, string currency)
    {
        return new Order
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = userId,
            ShippingAddress = shippingAddress,
            Subtotal = Money.Zero(currency),
            TaxAmount = Money.Zero(currency),
            ShippingCost = Money.Zero(currency),
            DiscountAmount = Money.Zero(currency),
            Total = Money.Zero(currency)
        };
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOrderStateException(Id, Status, "add items");
        }

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            product.ReserveStock(quantity);
            _items.Add(OrderItem.Create(Id, product.Id, product.Name, quantity, product.Price));
        }

        RecalculateTotals();
    }


    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOrderStateException(Id, Status, "remove items");
        }

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
        ?? throw new DomainException($"Product with ID {productId} not found in order.");
        _items.Remove(item);
        RecalculateTotals();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOrderStateException(Id, Status, "confirm");
        if (_items.Count == 0)
            throw new DomainException("Cannot confirm empty order.");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderPlacedEvent(Id, UserId, Total));
    }

    public void MarkAsPaid(Payment payment)
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOrderStateException(Id, Status, "mark as paid");

        Payment = payment;
        Status = OrderStatus.Processing;
        AddDomainEvent(new PaymentCompletedEvent(Id, payment.Id, Total));
    }

    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOrderStateException(Id, Status, "ship");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(Id, UserId, trackingNumber));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOrderStateException(Id, Status, "deliver");

        Status = OrderStatus.Delivered;
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new InvalidOrderStateException(Id, Status, "cancel");

        // Release reserved stock
        foreach (var item in _items)
            AddDomainEvent(new OrderCancelledEvent(Id, item.ProductId, item.Quantity));

        Status = OrderStatus.Cancelled;
        Notes = reason;
    }

    public void ApplyDiscount(Money discount)
    {
        if (discount.Amount > Subtotal.Amount)
            throw new DomainException("Discount cannot exceed subtotal.");

        DiscountAmount = discount;
        RecalculateTotals();
    }

    //Main Pricing Logic
    private void RecalculateTotals()
    {
        var currency = Subtotal.Currency;
        Subtotal = _items.Aggregate(
            Money.Zero(currency),
            (acc, item) => acc.Add(item.LineTotal));

        // Tax: 10% of (subtotal - discount)
        var taxableAmount = Subtotal.Subtract(DiscountAmount);
        TaxAmount = taxableAmount.Multiply(0.10m);

        Total = Subtotal
            .Add(TaxAmount)
            .Add(ShippingCost)
            .Subtract(DiscountAmount);
    }

    private static string GenerateOrderNumber() =>
        $"SK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

}