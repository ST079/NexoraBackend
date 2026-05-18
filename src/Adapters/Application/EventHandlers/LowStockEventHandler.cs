using MediatR;
using Microsoft.Extensions.Logging;
using NexoraBackend.Core.Events;

namespace NexoraBackend.Application.EventHandlers;

/// <summary>
/// Notifies admins when a product crosses the low-stock threshold.
/// In production, publish to a notification service or send an admin email.
/// </summary>
public class LowStockEventHandler : INotificationHandler<LowStockEvent>
{
    private readonly ILogger<LowStockEventHandler> _logger;

    public LowStockEventHandler(ILogger<LowStockEventHandler> logger) => _logger = logger;

    public Task Handle(LowStockEvent notification, CancellationToken ct)
    {
        _logger.LogWarning(
            "LOW STOCK ALERT: Product '{ProductName}' (ID: {ProductId}) has only {Stock} units remaining.",
            notification.ProductName, notification.ProductId, notification.CurrentStock);

        // TODO: plug in real notification channel (email, Slack, PushNotification, etc.)
        return Task.CompletedTask;
    }
}
