namespace NexoraBackend.Application.EventHandlers;

using MediatR;
using Microsoft.Extensions.Logging;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Core.Events;

public class OrderPlacedEventHandler : INotificationHandler<OrderPlacedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderPlacedEventHandler> _logger;

    public OrderPlacedEventHandler(IEmailService emailService, IUnitOfWork unitOfWork,
        ILogger<OrderPlacedEventHandler> logger)
    {
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(OrderPlacedEvent notification, CancellationToken ct)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(notification.OrderId, ct);
        if (order is null) return;

        var user = await _unitOfWork.Users.GetByIdAsync(order.UserId, ct);
        if (user is null) return;

        try
        {
            await _emailService.SendOrderConfirmationAsync(
                user.Email.Value, user.FirstName, order.OrderNumber, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}", notification.OrderId);
        }
    }
}
