using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;


namespace NexoraBackend.Application.Queries.Cart;

public record GetCartQuery(Guid UserId) : IRequest<Result<CartDto>>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<CartDto>> Handle(GetCartQuery query, CancellationToken ct)
    {
        var cart = await _unitOfWork.Carts.GetByUserAsync(query.UserId, ct);

        // Return an empty cart DTO if none exists yet — avoids null on the client
        if (cart is null)
        {
            var empty = new CartDto(Guid.Empty, query.UserId,
                new List<CartItemDto>(), 0, "USD", 0);
            return Result<CartDto>.Success(empty);
        }

        return Result<CartDto>.Success(_mapper.Map<CartDto>(cart));
    }
}