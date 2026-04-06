using System.Text.Json;
using HotelCore.Application.Common.Interfaces;

namespace HotelCore.Application.Common.Helpers;

/// <summary>
/// Extension methods for IEntityHistoryService to simplify common operations.
/// </summary>
public static class EntityHistoryExtensions
{
    /// <summary>
    /// Records multiple property changes for an entity in a single operation.
    /// </summary>
    public static async Task RecordChangesAsync<TEntity>(
        this IEntityHistoryService historyService,
        Guid entityId,
        Dictionary<string, (object? OldValue, object? NewValue)> changes,
        Guid? changedByUserId = null,
        string? changeDescription = null)
    {
        var entityType = typeof(TEntity).Name;

        foreach (var change in changes)
        {
            var oldValue = SerializeValue(change.Value.OldValue);
            var newValue = SerializeValue(change.Value.NewValue);

            await historyService.RecordChangeAsync(
                entityType: entityType,
                entityId: entityId,
                propertyName: change.Key,
                oldValue: oldValue,
                newValue: newValue,
                changedByUserId: changedByUserId,
                changeDescription: changeDescription
            );
        }
    }

    /// <summary>
    /// Records a single property change with automatic type handling.
    /// </summary>
    public static async Task RecordPropertyChangeAsync<TEntity>(
        this IEntityHistoryService historyService,
        Guid entityId,
        string propertyName,
        object? oldValue,
        object? newValue,
        Guid? changedByUserId = null,
        string? changeDescription = null)
    {
        var entityType = typeof(TEntity).Name;

        await historyService.RecordChangeAsync(
            entityType: entityType,
            entityId: entityId,
            propertyName: propertyName,
            oldValue: SerializeValue(oldValue),
            newValue: SerializeValue(newValue),
            changedByUserId: changedByUserId,
            changeDescription: changeDescription
        );
    }

    /// <summary>
    /// Serializes a value to string for storage in history.
    /// Handles primitives, enums, and complex objects.
    /// </summary>
    private static string? SerializeValue(object? value)
    {
        if (value == null)
            return null;

        if (value is string stringValue)
            return stringValue;

        if (value.GetType().IsPrimitive || value is decimal || value is DateTime || value is Guid)
            return value.ToString();

        if (value.GetType().IsEnum)
            return value.ToString();

        try
        {
            return JsonSerializer.Serialize(value);
        }
        catch
        {
            return value.ToString();
        }
    }
}
