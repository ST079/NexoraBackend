using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Enums;

namespace NexoraBackend.Application.Queries.Orders;

public record GetAllOrdersQuery(int Page = 1, int PageSize = 20, OrderStatus? Status = null)
    : IRequest<Result<PagedList<OrderDto>>>;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PagedList<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<PagedList<OrderDto>>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var paged = await _unitOfWork.Orders.GetAllPagedAsync(query.Page, query.PageSize, query.Status, ct);
        var mapped = new PagedList<OrderDto>(
            paged.Items.Select(o => _mapper.Map<OrderDto>(o)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Result<PagedList<OrderDto>>.Success(mapped);
    }
}