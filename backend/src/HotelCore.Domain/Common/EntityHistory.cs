namespace HotelCore.Domain.Common;

/// <summary>
/// Generic audit history record for tracking all changes to entities.
/// Provides a scalable, reusable pattern for entity change tracking.
/// </summary>
public class EntityHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Type name of the entity being tracked (e.g., "Order", "WorkerProfile").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the entity instance being tracked.
    /// </summary>
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// Name of the property that was changed.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Previous value before the change (stored as JSON or string).
    /// </summary>
    public string? OldValue { get; set; }
    
    /// <summary>
    /// New value after the change (stored as JSON or string).
    /// </summary>
    public string? NewValue { get; set; }
    
    /// <summary>
    /// Timestamp when the change occurred (UTC).
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of the user who made the change.
    /// </summary>
    public Guid? ChangedByUserId { get; set; }
    
    /// <summary>
    /// Optional description of the change operation.
    /// </summary>
    public string? ChangeDescription { get; set; }
}
