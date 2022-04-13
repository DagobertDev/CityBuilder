namespace CityBuilder.Core.Components;

public readonly record struct GrowthRate(float Value)
{
	public static implicit operator float(GrowthRate growthRate) => growthRate.Value;
	public static implicit operator GrowthRate(float growthRate) => new(growthRate);
}
