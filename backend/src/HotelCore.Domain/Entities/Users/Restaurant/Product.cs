using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Domain.Entities.Users.Restaurant
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }

        // Toggled by staff to temporarily hide items (e.g. out of stock).
        // Soft-delete covers permanent removal; IsAvailable is for temporary unavailability.
        public bool IsAvailable { get; set; } = true;

        public MyImage? Image { get; set; }
        public Guid? ImageId { get; set; }  // nullable — images are optional on products

        // null! because EF always populates this via the FK; suppresses the nullable warning.
        public ProductCategory Category { get; set; } = null!;
        public Guid ProductCategoryId { get; set; }
    }
}
