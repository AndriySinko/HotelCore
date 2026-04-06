using HotelCore.Application.Common.DTOs;
using HotelCore.Domain.Entities.Images;

namespace HotelCore.Application.Common.Mappers;

public static class ImageMapper
{
    public static MyImageGroupDto ToDto(this MyImageGroup group)
    {
        return new MyImageGroupDto(
            group.Id,
            group.Title,
            group.Position,
            group.Images.Select(ToDto).ToList());
    }

    public static MyImageDto ToDto(this MyImage image)
    {
        return new MyImageDto(
            image.Id,
            image.Url,
            image.Width,
            image.Height,
            image.SizeBytes,
            image.AspectRatio,
            image.Type);
    }

    public static IReadOnlyList<MyImageGroupDto> ToDto(this IEnumerable<MyImageGroup> groups)
        => groups.Select(ToDto).ToList();
}
