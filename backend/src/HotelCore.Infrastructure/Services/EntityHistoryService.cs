using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Domain.Common;

namespace HotelCore.Infrastructure.Services;

/// <summary>
/// Implementation of entity history tracking service.
/// Provides permanent audit trail for entity changes.
/// </summary>
public class EntityHistoryService : IEntityHistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public EntityHistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RecordChangeAsync(
        string entityType,
        Guid entityId,
        string propertyName,
        string? oldValue,
        string? newValue,
        Guid? changedByUserId = null,
        string? changeDescription = null)
    {
        if (oldValue == newValue)
        {
            return;
        }

        var history = new EntityHistory
        {
            EntityType = entityType,
            EntityId = entityId,
            PropertyName = propertyName,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedByUserId = changedByUserId,
            ChangeDescription = changeDescription,
            ChangedAt = DateTime.UtcNow
        };

        var dbContext = _unitOfWork as DbContext;
        if (dbContext != null)
        {
            await dbContext.Set<EntityHistory>().AddAsync(history);
        }
    }

    public async Task<IEnumerable<EntityHistory>> GetEntityHistoryAsync(string entityType, Guid entityId)
    {
        var dbContext = _unitOfWork as DbContext;
        if (dbContext == null)
        {
            return Enumerable.Empty<EntityHistory>();
        }

        return await dbContext.Set<EntityHistory>()
            .Where(h => h.EntityType == entityType && h.EntityId == entityId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<EntityHistory>> GetPropertyHistoryAsync(
        string entityType,
        Guid entityId,
        string propertyName)
    {
        var dbContext = _unitOfWork as DbContext;
        if (dbContext == null)
        {
            return Enumerable.Empty<EntityHistory>();
        }

        return await dbContext.Set<EntityHistory>()
            .Where(h => h.EntityType == entityType 
                     && h.EntityId == entityId 
                     && h.PropertyName == propertyName)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}
