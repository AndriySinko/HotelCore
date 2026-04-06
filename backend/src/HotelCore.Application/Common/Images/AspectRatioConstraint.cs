using System;
using System.Collections.Generic;
using System.Text;

namespace HotelCore.Application.Common.Images;

public abstract record AspectRatioConstraint
{
    public record Fixed(double Ratio) : AspectRatioConstraint
    {
        public static Fixed Square => new(1.0);
        public static Fixed Landscape16x9 => new(16.0 / 9.0);
        public static Fixed Landscape4x3 => new(4.0 / 3.0);
        public static Fixed Portrait9x16 => new(9.0 / 16.0);
        public static Fixed Portrait3x4 => new(3.0 / 4.0);
    }

    public record Range(double Min, double Max) : AspectRatioConstraint
    {
        public static Range Landscape => new(1.0, 3.0);
        public static Range Portrait => new(0.33, 1.0);
        public static Range Any => new(0.25, 4.0);
    }

    public bool IsValid(double aspectRatio) => this switch
    {
        Fixed f => Math.Abs(f.Ratio - aspectRatio) < 0.01,
        Range r => aspectRatio >= r.Min && aspectRatio <= r.Max,
        _ => true
    };

    public double GetTargetRatio(double originalRatio) => this switch
    {
        Fixed f => f.Ratio,
        Range r => Math.Clamp(originalRatio, r.Min, r.Max),
        _ => originalRatio
    };
}