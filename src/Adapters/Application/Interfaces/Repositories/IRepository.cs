using System.Linq.Expressions;
using NexoraBackend.Core.Domain;

namespace NexoraBackend.Application.Interfaces.Repositories;

/*
T = any entity type (User, Product, Order, etc.)
But it must inherit from BaseEntity
So you can reuse this repository for all tables.

instead of writing separate repository for each entity like:
IUserRepository
IProductRepository
we can have one generic repository that can handle all entities.

*/
public interface IRepository<T> where T : Base
{
    
    //Get one record by ID
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    //Get every record of that entity type
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    //filters Multiple records based on a condition
    //FindAsync(u => u.IsActive)
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    //filters one record based on a condition
    //FindOneAsync(u => u.Email == email)
    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);


    //insert new record
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    //modify existing record
    void Update(T entity);
    void Remove(T entity);

    //check if any record exists that matches the condition
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    //counts records
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);


    IQueryable<T> Query(); //A SQL query in progress (not executed yet)
    //a way to expose the database query pipeline directly 
    // so you can build LINQ queries outside the repository.

    /*
    var query = repo.Query()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name);

    here Sql is not executed until you call ,
    .ToList()
    .ToListAsync()
    .FirstOrDefault()
    .Count()

    we use this cause sometimes repo methods are not enough for complex queries and 
    we must add many methods to the repo like GetActiveUsers, GetProductsByCategory etc...

    with Query() method we ca build any query we want,
    repo.Query()
    .Where(u => u.IsActive)
    .Where(u => u.City == "Kathmandu")
    .Include(u => u.UserRoles)
    */
}