using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Enums
{
    public enum OrderStatus
    {
        InProgress,
        Received,
        Preparaing,
        OnTheWay,
        Delivered,
        Canceled
    }
}
