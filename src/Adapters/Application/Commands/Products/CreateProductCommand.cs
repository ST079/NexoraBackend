using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;



namespace NexoraBackend.Application.Commands.Products;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId,
    bool IsFeatured = false,
    List<string>? ImageUrls = null
) : IRequest<Result<ProductDto>>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.Id == cmd.CategoryId, ct);
        if (!categoryExists)
            return Result<ProductDto>.Failure(Error.NotFound("Category", cmd.CategoryId));

        var product = Product.Create(cmd.Name, cmd.Description, cmd.Price, cmd.Currency,
            cmd.StockQuantity, cmd.CategoryId);

        foreach (var url in cmd.ImageUrls ?? [])
            product.AddImage(url);

        await _unitOfWork.Products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Reload with category
        var created = await _unitOfWork.Products.FindOneAsync(
            p => p.Id == product.Id, ct);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(created!));
    }
}