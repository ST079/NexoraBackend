using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.Behaviors;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Commands.Orders;

//Command Query Responsibility Segregation (CQRS)

public record PlaceOrderCommand(
    Guid UserId,
    AddressDto ShippingAddress,
    string Currency = "USD"
) : IRequest<Result<OrderDto>>, ITransactionalCommand;
// returns result in OrderDto format, and needs transaction safety, so implements ITransactionalCommand

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ShippingAddress.Line1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShippingAddress.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShippingAddress.PostalCode).NotEmpty();
        RuleFor(x => x.ShippingAddress.Country).NotEmpty().Length(2);
    }
}

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public PlaceOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<Result<OrderDto>> Handle(PlaceOrderCommand command, CancellationToken ct)
    {
        // Get user's cart
        var cart = await _unitOfWork.Carts.FindOneAsync(c => c.UserId == command.UserId, ct);
        if (cart is null || cart.Items.Count == 0)
            return Result<OrderDto>.Failure(Error.Validation("Cart is empty."));

        // Build shipping address
        var addr = command.ShippingAddress;
        var shippingAddress = new Address(
            addr.Line1, addr.Line2, addr.City,
            addr.State, addr.PostalCode, addr.Country);

        // Create order
        //not using new Order() cause constructor is private, 
        // we want to enforce using factory method to ensure all invariants are met when creating order.
        var order = Order.Create(command.UserId, shippingAddress, command.Currency);

        // Transfer cart items to order
        foreach (var cartItem in cart.Items)
        {

            //why refetching product? cause we need to check if product is still active and get current price,
            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId, ct);
            if (product is null)
                return Result<OrderDto>.Failure(Error.NotFound("Product", cartItem.ProductId));
            if (!product.IsActive)
                return Result<OrderDto>.Failure(Error.Validation($"Product '{product.Name}' is no longer available."));

            // AddItem will check if requested quantity is available in stock and throw exception if not, 
            // which will trigger transaction rollback in TransactionBehavior.
            order.AddItem(product, cartItem.Quantity);
        }

        order.Confirm();
        cart.Clear();

        await _unitOfWork.Orders.AddAsync(order, ct);
        _unitOfWork.Carts.Update(cart);
        await _unitOfWork.SaveChangesAsync(ct);

        // Dispatch domain events
        foreach (var domainEvent in order.DomainEvents)
            await _mediator.Publish(domainEvent, ct);
        order.ClearDomainEvents();

        // Reload with navigation properties for mapping
        //Navigation property = a property that links one entity to another entity in a relationship

        //Navigation Property : Links one entity to another
        //EF Core : ORM that maps database tables → C# objects
        //in memory version : The object created in RAM after EF loads data from DB
        var created = await _unitOfWork.Orders.GetWithItemsAsync(order.Id, ct);
        
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(created!));
    }
}