using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Commands.Reviews;


public record SubmitReviewCommand(
    Guid UserId, Guid ProductId, int Rating, string Title, string Body
) : IRequest<Result<ReviewDto>>;

public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Body).NotEmpty().MinimumLength(20).MaximumLength(2000);
    }
}

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public SubmitReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    { _unitOfWork = unitOfWork; _mapper = mapper; _mediator = mediator; }

    public async Task<Result<ReviewDto>> Handle(SubmitReviewCommand cmd, CancellationToken ct)
    {
        if (!await _unitOfWork.Products.ExistsAsync(p => p.Id == cmd.ProductId, ct))
            return Result<ReviewDto>.Failure(Error.NotFound("Product", cmd.ProductId));

        if (await _unitOfWork.Reviews.HasUserReviewedProductAsync(cmd.UserId, cmd.ProductId, ct))
            return Result<ReviewDto>.Failure(
                Error.Conflict("You have already submitted a review for this product."));

        // Verified purchase: user has a Delivered order containing this product
        var isVerified = await _unitOfWork.Orders.ExistsAsync(
            o => o.UserId == cmd.UserId
                 && o.Status == OrderStatus.Delivered
                 && o.Items.Any(i => i.ProductId == cmd.ProductId), ct);

        var review = Review.Create(cmd.ProductId, cmd.UserId,
            cmd.Rating, cmd.Title, cmd.Body, isVerified);

        await _unitOfWork.Reviews.AddAsync(review, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        foreach (var de in review.DomainEvents) await _mediator.Publish(de, ct);
        review.ClearDomainEvents();

        var created = await _unitOfWork.Reviews.GetByIdAsync(review.Id, ct);
        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(created!));
    }
}