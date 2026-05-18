using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Exceptions;

namespace NexoraBackend.Application.Commands.Reviews;

public record ModerateReviewCommand(Guid ReviewId, bool Approve, string? RejectionReason)
    : IRequest<Result<ReviewDto>>;

public class ModerateReviewCommandHandler : IRequestHandler<ModerateReviewCommand, Result<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public ModerateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    { 
        _unitOfWork = unitOfWork; 
        _mapper = mapper; 
        _mediator = mediator; 
    }

    public async Task<Result<ReviewDto>> Handle(ModerateReviewCommand cmd, CancellationToken ct)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(cmd.ReviewId, ct);
        if (review is null) return Result<ReviewDto>.Failure(Error.NotFound("Review", cmd.ReviewId));

        try
        {
            if (cmd.Approve) review.Approve();
            else review.Reject(cmd.RejectionReason ?? "Does not meet community guidelines.");
        }
        catch (DomainException ex) { return Result<ReviewDto>.Failure(Error.Validation(ex.Message)); }

        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync(ct);

        foreach (var de in review.DomainEvents) await _mediator.Publish(de, ct);
        review.ClearDomainEvents();

        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
}