namespace CityBuilder.Core.Components.Needs;

public readonly record struct TirednessRate(float Value)
{
	public static implicit operator float(TirednessRate rate) => rate.Value;
	public static implicit operator TirednessRate(float value) => new(value);
}
