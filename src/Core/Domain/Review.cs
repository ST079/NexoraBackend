using NexoraBackend.Core.Enums;
using NexoraBackend.Core.Events;
using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

/// Review is a full auditable entity because it has its own identity,
/// can be edited/deleted, and triggers moderation events.
/// Rating is a value object enforcing the 1–5 constraint.
/// One user can only have one approved review per product (enforced via
/// a unique index on UserId + ProductId in the DB configuration).
/// </summary>
public class Review : Auditable
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Rating Rating { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;
    public bool IsVerifiedPurchase { get; private set; }

    private readonly List<ReviewHelpfulVote> _helpfulVotes = [];
    public IReadOnlyCollection<ReviewHelpfulVote> HelpfulVotes => _helpfulVotes.AsReadOnly();

    public int HelpfulCount => _helpfulVotes.Count(v => v.IsHelpful);
    public int NotHelpfulCount => _helpfulVotes.Count(v => !v.IsHelpful);

    private Review() { }

    public static Review Create(Guid productId, Guid userId,
        int rating, string title, string body, bool isVerifiedPurchase = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Review title is required.");
        if (string.IsNullOrWhiteSpace(body))
            throw new DomainException("Review body is required.");
        if (body.Length < 20)
            throw new DomainException("Review body must be at least 20 characters.");

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = new Rating(rating),
            Title = title.Trim(),
            Body = body.Trim(),
            IsVerifiedPurchase = isVerifiedPurchase
        };

        review.AddDomainEvent(new ReviewSubmittedEvent(review.Id, productId, userId));
        return review;
    }

    public void Approve()
    {
        if (Status == ReviewStatus.Approved)
            throw new DomainException("Review is already approved.");
        Status = ReviewStatus.Approved;
        AddDomainEvent(new ReviewApprovedEvent(Id, ProductId));
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason must be provided when rejecting a review.");
        Status = ReviewStatus.Rejected;
    }

    public void Update(int rating, string title, string body)
    {
        if (Status == ReviewStatus.Approved)
            throw new DomainException("Approved reviews cannot be edited. Delete and resubmit.");

        Rating = new Rating(rating);
        Title = title.Trim();
        Body = body.Trim();
        Status = ReviewStatus.Pending; // Re-queue for moderation
    }

    public void VoteHelpful(Guid votingUserId, bool isHelpful)
    {
        if (votingUserId == UserId)
            throw new DomainException("You cannot vote on your own review.");

        var existing = _helpfulVotes.FirstOrDefault(v => v.UserId == votingUserId);
        if (existing != null)
        {
            // Toggle existing vote
            existing.Update(isHelpful);
            return;
        }
        _helpfulVotes.Add(new ReviewHelpfulVote(Id, votingUserId, isHelpful));
    }
}