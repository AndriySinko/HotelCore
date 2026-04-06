using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Images;

public record ImageSizeDefinition(
    ImageSizeType Type,
    int? Width,
    int? Height,
    AspectRatioConstraint? AspectRatio = null)
{
    public AspectRatioConstraint EffectiveAspectRatio => AspectRatio ?? AspectRatioConstraint.Range.Any;
}

