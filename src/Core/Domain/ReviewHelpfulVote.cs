namespace NexoraBackend.Core.Domain;

/// Tracks whether a user found a review helpful or not.
/// Owned by Review — no independent lifecycle.
/// </summary>
public class ReviewHelpfulVote : Base
{
    public Guid ReviewId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsHelpful { get; private set; }
    public DateTime VotedAt { get; private set; }

    private ReviewHelpfulVote() { }

    public ReviewHelpfulVote(Guid reviewId, Guid userId, bool isHelpful)
    {
        ReviewId = reviewId;
        UserId = userId;
        IsHelpful = isHelpful;
        VotedAt = DateTime.UtcNow;
    }

    public void Update(bool isHelpful)
    {
        IsHelpful = isHelpful;
        VotedAt = DateTime.UtcNow;
    }
}