using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess;
using myshop.Entities.Models.Interfaces;
using Repositories.Interfaces;

namespace Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null, bool ignoreQueryFilters = false)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id, bool ignoreQueryFilters = false)
    {
        if (ignoreQueryFilters)
        {
            return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool ignoreQueryFilters = false)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool ignoreQueryFilters = false)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        IQueryable<T> query = _dbSet.AsNoTracking();

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        int totalCount = await query.CountAsync();

        if (include != null)
        {
            query = include(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public async Task UpdateAsync(T entity) =>
        _dbSet.Update(entity);

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = false;
            softDeletable.DeletedAt = null;
            _dbSet.Update(entity);
        }
    }
}