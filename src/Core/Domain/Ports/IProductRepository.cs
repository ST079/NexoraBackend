using NexoraBackend.Core.Domain.Entities;
namespace NexoraBackend.Core.Domain.Ports;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();

    Task<Product?> GetProductByIdAsync(Guid id);

    Task CreateProductAsync(Product product);

    Task UpdateProductAsync(Product product);

    Task DeleteProductAsync(Guid id);
}