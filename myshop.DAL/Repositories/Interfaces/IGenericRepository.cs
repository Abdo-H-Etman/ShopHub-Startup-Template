using System.Linq.Expressions;

namespace Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, CancellationToken cancellationToken = default);
}
