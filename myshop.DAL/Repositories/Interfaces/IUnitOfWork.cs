using myshop.Entities.Models;

namespace Repositories.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Category> Categories { get; }
    IGenericRepository<OrderHeader> OrderHeaders { get; }
    IGenericRepository<OrderDetail> OrderDetails { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<ShoppingCart> ShoppingCarts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}