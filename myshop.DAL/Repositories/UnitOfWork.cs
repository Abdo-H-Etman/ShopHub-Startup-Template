using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using myshop.DataAccess;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IGenericRepository<Product> Products => _serviceProvider.GetRequiredService<IGenericRepository<Product>>();
    public IGenericRepository<Category> Categories => _serviceProvider.GetRequiredService<IGenericRepository<Category>>();
    public IGenericRepository<OrderHeader> OrderHeaders => _serviceProvider.GetRequiredService<IGenericRepository<OrderHeader>>();
    public IGenericRepository<OrderDetail> OrderDetails => _serviceProvider.GetRequiredService<IGenericRepository<OrderDetail>>();
    public IGenericRepository<Review> Reviews => _serviceProvider.GetRequiredService<IGenericRepository<Review>>();
    public IGenericRepository<ShoppingCart> ShoppingCarts => _serviceProvider.GetRequiredService<IGenericRepository<ShoppingCart>>();

    public async Task<int> SaveChangesAsync()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return await _context.SaveChangesAsync();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}