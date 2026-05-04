using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    /// <summary>Root aggregate for a guest's room-service order.</summary>
    public class Order : BaseEntity
    {
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public required OrderStatus Status { get; set; }
        public Payment? Payment { get; set; }

        // Null when the JWT sub can't be resolved to a registered guest
        // (e.g. walk-in, or a token whose sub isn't a valid Guid).
        public Guid? GuestId { get; set; }
    }
}
