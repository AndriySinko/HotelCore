using HotelCore.Domain.Common;
namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class OrderItem : BaseEntity
    {
        public Order Order { get; set; }
        public Guid OrderId { get; set; }

        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public required decimal PricePerUnit { get; set; }
        public required int Quantity { get; set; }
        public string? SpecialRequest { get; set; }
    }
}
