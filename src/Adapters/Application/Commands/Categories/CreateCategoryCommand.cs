// src/Adapters/NexoraBackend.Application/Commands/Categories/CreateCategoryCommand.cs
using AutoMapper;
using FluentValidation;
using MediatR;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Application.Interfaces.Repositories;
using NexoraBackend.Common.Models;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Commands.Categories;

public record CreateCategoryCommand(
    string Name, string Description, Guid? ParentId, string? ImageUrl, int SortOrder = 0
) : IRequest<Result<CategoryDto>>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand cmd, CancellationToken ct)
    {
        if (cmd.ParentId.HasValue && !await _uow.Categories.ExistsAsync(c => c.Id == cmd.ParentId.Value, ct))
            return Result<CategoryDto>.Failure(Error.NotFound("Category", cmd.ParentId.Value));

        var slug = Slug.Create(cmd.Name).Value;
        if (await _uow.Categories.SlugExistsAsync(slug, ct: ct))
            return Result<CategoryDto>.Failure(Error.Conflict($"Category slug '{slug}' already exists."));

        var category = Category.Create(cmd.Name, cmd.Description, cmd.ParentId, cmd.ImageUrl, cmd.SortOrder);
        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
    }
}