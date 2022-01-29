namespace CityBuilder.Core.Components.AI;

public readonly record struct Speed(float Value)
{
	public static implicit operator float(Speed speed) => speed.Value;
	public static implicit operator Speed(float value) => new(value);
}
