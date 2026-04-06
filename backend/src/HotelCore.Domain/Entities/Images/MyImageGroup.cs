using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Images;

public class MyImageGroup : BaseEntity
{
    public string? Title { get; set; }
    public int Position { get; set; }
    public MyImageType Type { get; set; }

    public ICollection<MyImage> Images { get; init; } = [];

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public Guid? WorkerPortfolioItemId { get; set; }
    public WorkerPortfolioItem? WorkerPortfolioItem { get; set; }

    public MyImage? GetBySize(ImageSizeType sizeType) 
        => Images.FirstOrDefault(i => i.Type == sizeType);

    public MyImage? Thumbnail => GetBySize(ImageSizeType.Thumbnail);
    public MyImage? Small => GetBySize(ImageSizeType.Small);
    public MyImage? Medium => GetBySize(ImageSizeType.Medium);
    public MyImage? Large => GetBySize(ImageSizeType.Large);
}
