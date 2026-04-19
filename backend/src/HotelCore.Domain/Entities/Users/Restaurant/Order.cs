using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class Order: BaseEntity
    {
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public required OrderStatus Status { get; set; }
        public Payment? Payment { get; set; }
        public Guid? GuestId { get; set; }
    }
}
