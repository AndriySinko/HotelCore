using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public MyImage? Image { get; set; }
        public Guid? ImageId { get; set; }

        public ProductCategory Category { get; set; } = null!;
        public Guid ProductCategoryId { get; set; }
    }
}
