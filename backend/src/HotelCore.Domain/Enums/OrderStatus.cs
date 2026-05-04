using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Enums
{
    public enum OrderStatus
    {
        InProgress,
        Received,
        Preparaing,  // typo — fixing it requires a migration to update stored enum values
        OnTheWay,
        Delivered,
        Canceled
    }
}
