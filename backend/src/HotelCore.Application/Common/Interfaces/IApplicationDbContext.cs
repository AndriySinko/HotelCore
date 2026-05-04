using Microsoft.EntityFrameworkCore;
using HotelCore.Domain.Entities.Users.Restaurant;

namespace HotelCore.Application.Common.Interfaces;

/// <summary>
/// Application-layer abstraction over the restaurant slice of the database.
/// All DbSets here have the soft-delete global query filter applied by the EF configuration —
/// deleted records are excluded unless you explicitly call <c>IgnoreQueryFilters()</c>.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
