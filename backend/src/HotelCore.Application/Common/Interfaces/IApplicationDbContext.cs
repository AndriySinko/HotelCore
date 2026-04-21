using Microsoft.EntityFrameworkCore;
using HotelCore.Domain.Entities.Users.Restaurant;

namespace HotelCore.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
