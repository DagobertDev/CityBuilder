namespace CityBuilder.Core.Components;

public readonly record struct Rotation(int Value)
{
	public static implicit operator int(Rotation rotation) => rotation.Value;
	public static implicit operator Rotation(int value) => new(value);
}
