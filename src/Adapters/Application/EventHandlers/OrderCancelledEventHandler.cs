using MediatR;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Core.Events;

namespace NexoraBackend.Application.EventHandlers;


public class OrderCancelledEventHandler : INotificationHandler<OrderCancelledEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderCancelledEventHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(OrderCancelledEvent notification, CancellationToken ct)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(notification.ProductId, ct);
        if (product is null) return;

        product.ReleaseStock(notification.QuantityToRelease);
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
