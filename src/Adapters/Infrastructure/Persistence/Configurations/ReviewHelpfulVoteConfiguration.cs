using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Infrastructure.Persistence.Configurations;

public class ReviewHelpfulVoteConfiguration : IEntityTypeConfiguration<ReviewHelpfulVote>
{
    public void Configure(EntityTypeBuilder<ReviewHelpfulVote> builder)
    {
        builder.ToTable("review_helpful_votes");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.VotedAt).IsRequired();

        // One vote per user per review
        builder.HasIndex(v => new { v.ReviewId, v.UserId })
            .IsUnique().HasDatabaseName("idx_votes_review_user");
    }
}