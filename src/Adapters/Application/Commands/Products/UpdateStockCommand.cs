// src/Adapters/ECommerce.Application/Commands/Products/UpdateStockCommand.cs
using AutoMapper;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;

namespace NexoraBackend.Application.Commands.Products;

public record UpdateStockCommand(Guid ProductId, int Quantity) : IRequest<Result<ProductDto>>;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateStockCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork; _mapper = mapper; _mediator = mediator;
    }

    public async Task<Result<ProductDto>> Handle(UpdateStockCommand cmd, CancellationToken ct)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(cmd.ProductId, ct);
        if (product is null) return Result<ProductDto>.Failure(Error.NotFound("Product", cmd.ProductId));

        product.UpdateStock(cmd.Quantity);
        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        foreach (var ev in product.DomainEvents) await _mediator.Publish(ev, ct);
        product.ClearDomainEvents();

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}