using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Orders;

public record GetOrderQuery(Guid OrderId, Guid RequestingUserId)
    : IRequest<Result<OrderDto>>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IMapper _mapper;

    public GetOrderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<OrderDto>> Handle(GetOrderQuery query, CancellationToken ct)
    {
        var order = await _unitOfWork.Orders.GetWithItemsAsync(query.OrderId, ct);
        if (order is null)
            return Result<OrderDto>.Failure(Error.NotFound("Order", query.OrderId));

        // Users can only see their own orders; admins can see all (role check in API layer)
        if (order.UserId != query.RequestingUserId)
            return Result<OrderDto>.Failure(Error.Unauthorized());

        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}