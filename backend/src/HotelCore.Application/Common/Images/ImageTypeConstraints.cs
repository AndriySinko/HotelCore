using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Common.Images;

public record ImageTypeConstraints(
    IReadOnlyList<ImageSizeDefinition> Sizes,
    int MaxPixelDimension);
