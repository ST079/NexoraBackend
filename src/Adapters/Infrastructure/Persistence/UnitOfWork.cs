namespace NexoraBackend.Infrastructure.Persistence;


using Microsoft.EntityFrameworkCore.Storage;
using NexoraBackend.Application.Interfaces.Repositories;

public class UnitOfWork : IUnitOfWork
{
    //Treat multiple repository operations as a single transaction
    private readonly AppDbContext _context;

    //Transaction object to manage commit/rollback
    private IDbContextTransaction? _transaction; //allows beginning a transaction, committing it, or rolling it back if something goes wrong

    //Repo Access layer. We inject all repos here so they can share the same DbContext/transaction
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public IUserRepository Users { get; }
    public ICartRepository Carts { get; }
    public ICategoryRepository Categories { get; }
    public IPaymentRepository Payments { get; }
    public IReviewRepository Reviews { get; }
    public ICouponRepository Coupons { get; }

    public UnitOfWork(AppDbContext context,
        IProductRepository products, IOrderRepository orders,
        IUserRepository users, ICartRepository carts,
        ICategoryRepository categories, IPaymentRepository payments,
        IReviewRepository reviews, ICouponRepository coupons)
    {
        _context = context;
        Products = products;
        Orders = orders;
        Users = users;
        Carts = carts;
        Categories = categories;
        Payments = payments;
        Reviews = reviews;
        Coupons = coupons;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct); //save changes to db

    public async Task BeginTransactionAsync(CancellationToken ct = default) =>
        _transaction = await _context.Database.BeginTransactionAsync(ct); //opens a DB transaction, 
        // allowing multiple operations to be treated as a single unit of work


    //This ensures data is permanently stored
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        await _transaction!.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    //Cancels everything that happened inside transaction
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        await _transaction!.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }


    // after the request is done, we dispose the transaction and DbContext to free resources
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
    /*
    Without Dispose:
    connections pile up
    DB pool exhaustion happens
    API slows down or crashes
    */
}