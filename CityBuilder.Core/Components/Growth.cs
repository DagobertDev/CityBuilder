namespace CityBuilder.Core.Components;

public readonly record struct Growth(float Value)
{
	public static implicit operator float(Growth growth) => growth.Value;
	public static implicit operator Growth(float growth) => new(growth);
}
