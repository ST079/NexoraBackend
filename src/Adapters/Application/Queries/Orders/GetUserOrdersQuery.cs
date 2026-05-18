
using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Queries.Orders;

public record GetUserOrdersQuery(Guid UserId, int Page = 1, int PageSize = 10)
    : IRequest<Result<PagedList<OrderDto>>>;

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, Result<PagedList<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    { _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<Result<PagedList<OrderDto>>> Handle(GetUserOrdersQuery query, CancellationToken ct)
    {
        var paged = await _unitOfWork.Orders.GetByUserAsync(query.UserId, query.Page, query.PageSize, ct);
        var mapped = new PagedList<OrderDto>(
            paged.Items.Select(o => _mapper.Map<OrderDto>(o)).ToList(),
            paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Result<PagedList<OrderDto>>.Success(mapped);
    }
}
