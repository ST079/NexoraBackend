using MediatR;
using Microsoft.Extensions.Logging;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Core.Events;

namespace NexoraBackend.Application.EventHandlers;

/// <summary>
/// Sends the shipping confirmation email with tracking number
/// when an order transitions to Shipped status.
/// </summary>
public class OrderShippedEventHandler : INotificationHandler<OrderShippedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<OrderShippedEventHandler> _logger;

    public OrderShippedEventHandler(IEmailService emailService, IUnitOfWork uow,
        ILogger<OrderShippedEventHandler> logger)
    {
        _emailService = emailService;
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(OrderShippedEvent notification, CancellationToken ct)
    {
        var order = await _uow.Orders.GetWithItemsAsync(notification.OrderId, ct);
        if (order is null) return;

        var user = await _uow.Users.GetByIdAsync(notification.UserId, ct);
        if (user is null) return;

        try
        {
            await _emailService.SendOrderShippedAsync(
                user.Email.Value, user.FirstName, order.OrderNumber,
                notification.TrackingNumber, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send shipment email for order {OrderId}", notification.OrderId);
        }
    }
}