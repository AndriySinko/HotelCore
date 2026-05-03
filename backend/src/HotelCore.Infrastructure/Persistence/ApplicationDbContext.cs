// main EF Core database context — all queries and writes go through here
// extends IdentityDbContext so ASP.NET Identity tables (users, roles, claims) are included automatically
// implements IUnitOfWork so the application layer can call SaveChangesAsync without a direct EF reference
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Restaurant;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // reception module tables
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Payment> Payments => Set<Payment>();

    // cleaning module table
    public DbSet<CleaningTask> CleaningTasks => Set<CleaningTask>();

    // restaurant module tables — owned by Person B
    public DbSet<FoodOrder> FoodOrders => Set<FoodOrder>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    // staff management tables
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<ShiftChangeRequest> ShiftChangeRequests => Set<ShiftChangeRequest>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // intercept any delete operations and convert them to soft deletes
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    public void MarkUnchanged<T>(T entity) where T : class
        => Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;

    // soft delete — instead of removing the row, we set IsDeleted = true and record the timestamp
    // this means deleted records are still in the database and can be recovered if needed
    private void HandleSoftDelete()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Deleted && e.Entity is IBaseEntity);

        foreach (var entry in entries)
        {
            var entity = (IBaseEntity)entry.Entity;
            entry.State = EntityState.Modified;
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // automatically picks up all IEntityTypeConfiguration classes from this assembly
        // each module has its own configuration files in Persistence/Configurations/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
