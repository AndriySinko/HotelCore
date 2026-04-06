using HotelCore.Domain.Common;

namespace HotelCore.Application.Common.Interfaces;

/// <summary>
/// Service for tracking and managing entity change history.
/// Provides a scalable, reusable pattern for auditing entity changes.
/// </summary>
public interface IEntityHistoryService
{
    /// <summary>
    /// Records a change to an entity property.
    /// </summary>
    /// <param name="entityType">Type name of the entity (e.g., "Order")</param>
    /// <param name="entityId">ID of the entity instance</param>
    /// <param name="propertyName">Name of the property that changed</param>
    /// <param name="oldValue">Previous value</param>
    /// <param name="newValue">New value</param>
    /// <param name="changedByUserId">User who made the change</param>
    /// <param name="changeDescription">Optional description</param>
    Task RecordChangeAsync(
        string entityType,
        Guid entityId,
        string propertyName,
        string? oldValue,
        string? newValue,
        Guid? changedByUserId = null,
        string? changeDescription = null);

    /// <summary>
    /// Gets the complete history of changes for a specific entity.
    /// </summary>
    /// <param name="entityType">Type name of the entity</param>
    /// <param name="entityId">ID of the entity instance</param>
    /// <returns>List of all historical changes</returns>
    Task<IEnumerable<EntityHistory>> GetEntityHistoryAsync(string entityType, Guid entityId);

    /// <summary>
    /// Gets the history of changes for a specific property of an entity.
    /// </summary>
    /// <param name="entityType">Type name of the entity</param>
    /// <param name="entityId">ID of the entity instance</param>
    /// <param name="propertyName">Name of the property</param>
    /// <returns>List of changes for the specified property</returns>
    Task<IEnumerable<EntityHistory>> GetPropertyHistoryAsync(
        string entityType,
        Guid entityId,
        string propertyName);
}
