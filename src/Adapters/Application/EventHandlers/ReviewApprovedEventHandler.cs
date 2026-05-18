using MediatR;
using NexoraBackend.Application.Interfaces.Services;
using NexoraBackend.Core.Events;

namespace NexoraBackend.Application.EventHandlers;


/// When a review is approved, invalidate cached product data so the
/// AverageRating recalculates on next request.
public class ReviewApprovedEventHandler : INotificationHandler<ReviewApprovedEvent>
{
    private readonly ICacheService _cache;
    public ReviewApprovedEventHandler(ICacheService cache) => _cache = cache;

    public async Task Handle(ReviewApprovedEvent notification, CancellationToken ct)
    {
        await _cache.RemoveAsync($"product:{notification.ProductId}", ct);
        await _cache.RemoveByPrefixAsync($"reviews:product:{notification.ProductId}", ct);
    }
}