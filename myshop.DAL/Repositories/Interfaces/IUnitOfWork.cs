using myshop.Entities.Models;

namespace Repositories.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Category> Categories { get; }
    Task<int> SaveChangesAsync();
}