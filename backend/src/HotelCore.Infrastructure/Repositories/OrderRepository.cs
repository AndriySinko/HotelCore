using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Extensions;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Orders.Models;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class OrderRepository(ApplicationDbContext db) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await db.Orders
            .Include(o => o.Client)
                .ThenInclude(c => c!.User)
            .Include(o => o.CreatedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<MyImageGroup>> GetOrderImagesAsync(
        Guid orderId,
        PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        return await db.MyImageGroups
            .AsNoTracking()
            .Include(g => g.Images)
            .Where(g => g.OrderId == orderId)
            .OrderBy(g => g.Position)
            .Paginate(pageRequest)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(
        OrdersFilter filter,
        PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Client)
                .ThenInclude(c => c!.User)
            .AsQueryable();

        if (filter.Type.HasValue)
            query = query.Where(o => o.Type == filter.Type.Value);

        if (filter.CreatedByUserId.HasValue)
            query = query.Where(o => o.CreatedByUserId == filter.CreatedByUserId.Value);

        if (filter.ClientUserId.HasValue)
            query = query.Where(o => o.Client != null && o.Client.UserId == filter.ClientUserId.Value);

        if (!string.IsNullOrEmpty(filter.GuestAccessToken))
            query = query.Where(o => o.Client != null
                && o.Client.UserId == null
                && o.Client.GuestAccessToken == filter.GuestAccessToken);

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);

        if (filter.ExcludeStatus.HasValue)
            query = query.Where(o => o.Status != filter.ExcludeStatus.Value);

        if (filter.IsHidden.HasValue)
            query = query.Where(o => o.IsHidden == filter.IsHidden.Value);

        Expression<Func<Order, object?>> keySelector = filter.SortBy switch
        {
            OrderSortBy.Price => o => o.Price,
            OrderSortBy.Status => o => o.Status,
            OrderSortBy.Title => o => o.Title,
            OrderSortBy.UpdatedAt => o => o.UpdatedAt,
            _ => o => o.CreatedAt
        };

        query = filter.SortDescending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);

        return await query
            .Paginate(pageRequest)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetSubOrdersAsync(
        Guid parentOrderId,
        CancellationToken cancellationToken)
    {
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.ParentOrderId == parentOrderId)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(Order order)
    {
        db.Orders.Add(order);
    }

    public void Update(Order order)
    {
        db.Orders.Update(order);
    }

    public async Task<int> MoveGuestOrdersToUserAsync(string guestAccessToken, Guid userId, CancellationToken cancellationToken)
    {
        var orderClients = await db.OrderClients
            .Include(oc => oc.Order)
            .Where(oc => oc.GuestAccessToken == guestAccessToken && oc.UserId == null)
            .ToListAsync(cancellationToken);

        foreach (var client in orderClients)
        {
            client.UserId = userId;
            client.GuestAccessToken = null;

            if (client.Order is not null)
            {
                client.Order.UpdatedAt = DateTime.UtcNow;
            }
        }

        return orderClients.Count;
    }
}
