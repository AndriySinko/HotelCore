// This file contains code for ShiftConflictService.
using HotelCore.Domain.Entities.StaffManagement;

namespace HotelCore.Domain.Services;



public static class ShiftConflictService
{
    
    
    
    
    public static bool HasConflict(
        DateTime proposedDate,
        DateTime proposedStart,
        DateTime proposedEnd,
        IEnumerable<Shift> existing)
    {
        var newStart = proposedStart;
        var newEnd   = proposedEnd;
        if (newEnd <= newStart) newEnd = newEnd.AddDays(1);

        foreach (var shift in existing)
        {
            var existStart = shift.StartTime;
            var existEnd   = shift.EndTime;
            if (existEnd <= existStart) existEnd = existEnd.AddDays(1);

            if (newStart < existEnd && existStart < newEnd)
                return true;
        }
        return false;
    }
}
