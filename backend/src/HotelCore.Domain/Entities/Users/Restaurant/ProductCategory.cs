using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class ProductCategory: BaseEntity
    {
        public required string Name { get; set; }
        public ICollection<Product> Products { get; set;} = new List<Product>();
    }
}
