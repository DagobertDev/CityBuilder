namespace CityBuilder.Core.Components.Needs;

public readonly record struct HungerRate(float Value)
{
	public static implicit operator float(HungerRate rate) => rate.Value;
	public static implicit operator HungerRate(float value) => new(value);
}
