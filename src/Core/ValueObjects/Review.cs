using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Core.ValueObjects;


public sealed record Review
{
    public int Rating { get; }
    public string Comment { get; }

    private Review() { }

    public Review(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Review comment cannot be empty.");

        Rating = rating;
        Comment = comment;
    }
}