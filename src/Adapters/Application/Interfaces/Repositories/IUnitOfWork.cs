namespace NexoraBackend.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
   //all repo here to ensure they share the same DbContext/transaction
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    IUserRepository Users { get; }
    ICartRepository Carts { get; }
    ICategoryRepository Categories { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }   
    ICouponRepository Coupons { get; }      

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    //Transaction Methods
    //it gives us control to treat multiple repository operations as a single transaction
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

}