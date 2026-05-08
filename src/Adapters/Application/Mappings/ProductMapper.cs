using NexoraBackend.Application.Entities;
using NexoraBackend.Core.Domain.Entities;

namespace NexoraBackend.Adapters.Application.Mappings;

public static class ProductMapper
{
    public static Product ToDomain(ProductEntity entity)
    {
        return new Product
        {
            ProductId = entity.ProductId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            Category = entity.Category,
            Brand = entity.Brand,
            ImageUrls = entity.ImageUrls,
        };
    }

    public static ProductEntity ToEntity(Product product)
    {
        return new ProductEntity
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            Brand = product.Brand,
            ImageUrls = product.ImageUrls,
        };
    }
}