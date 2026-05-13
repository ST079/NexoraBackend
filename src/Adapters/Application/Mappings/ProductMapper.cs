using NexoraBackend.Application.DTOs.Responses;
using NexoraBackend.Application.Entities;
using NexoraBackend.Application.DTOs.Inputs;
using Riok.Mapperly.Abstractions;
using NexoraBackend.Core.Domain.Entities;


namespace NexoraBackend.Application.Mappings;

[Mapper]
public partial class ProductMapper
{
    // DTO → Domain
    public partial Product ToDomain(ProductDto dto);

    // Domain → Entity
    public partial ProductEntity ToEntity(Product domain);

    // Entity → Domain
    public partial Product ToDomain(ProductEntity entity);

    // Domain → DTO
    public partial ProductResponseDto ToDto(Product domain);
}