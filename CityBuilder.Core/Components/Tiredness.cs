namespace CityBuilder.Core.Components;

public readonly record struct Tiredness(float Value)
{
	public static implicit operator float(Tiredness tiredness) => tiredness.Value;
	public static implicit operator Tiredness(float value) => new(value);
}
