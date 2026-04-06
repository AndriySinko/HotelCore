using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Entities.Companies;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Locations;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Entities.Payments;
using HotelCore.Domain.Entities.Reviews;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Entities.Seekers;

namespace HotelCore.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkerProfile> WorkerProfiles => Set<WorkerProfile>();
    public DbSet<SeekerProfile> SeekerProfiles => Set<SeekerProfile>();
    public DbSet<WorkerServiceListing> WorkerServiceListings => Set<WorkerServiceListing>();
    public DbSet<WorkerPortfolioItem> WorkerPortfolioItems => Set<WorkerPortfolioItem>();
    public DbSet<WorkerMedia> WorkerMedia => Set<WorkerMedia>();
    public DbSet<WorkerCategory> WorkerCategories => Set<WorkerCategory>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewMedia> ReviewMedia => Set<ReviewMedia>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderClient> OrderClients => Set<OrderClient>();
    public DbSet<MyImage> MyImages => Set<MyImage>();
    public DbSet<MyImageGroup> MyImageGroups => Set<MyImageGroup>();
    public DbSet<EntityHistory> EntityHistories => Set<EntityHistory>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Ensure Identity tables use snake_case if configured, though ApplyConfigurations or naming convention should handle it
    }
}
