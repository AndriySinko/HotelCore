using HotelCore.Domain.Common;
using HotelCore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class Payment: BaseEntity
    {
        public PaymentMethod Method { get; set; }

        public Order Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
