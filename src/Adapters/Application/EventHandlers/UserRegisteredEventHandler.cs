using MediatR;
using Microsoft.Extensions.Logging;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Events;

namespace NexoraBackend.Application.EventHandlers;

/// <summary>
/// Triggered after a new user registers. Creates an empty cart for them immediately
/// so the first AddToCart call never has to check-and-create.
/// </summary>
public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IUnitOfWork unitOfWork, ILogger<UserRegisteredEventHandler> logger)
    { _unitOfWork = unitOfWork; _logger = logger; }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken ct)
    {
        var cart = Cart.Create(notification.UserId);
        await _unitOfWork.Carts.AddAsync(cart, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Empty cart provisioned for new user {UserId}.", notification.UserId);
    }
}