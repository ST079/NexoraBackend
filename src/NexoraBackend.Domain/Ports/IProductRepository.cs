using NexoraBackend.Domain.Entities;
namespace NexoraBackend.Domain.Ports;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();

    Task<Product?> GetProductByIdAsync(Guid id);

    Task CreateProductAsync(Product product);

    Task UpdateProductAsync(Product product);

    Task DeleteProductAsync(Guid id);
}