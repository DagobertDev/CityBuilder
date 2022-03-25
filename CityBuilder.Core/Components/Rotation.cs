namespace CityBuilder.Core.Components;

/// <summary>
///     Rotation of an object in degree
/// </summary>
public readonly record struct Rotation(int Value)
{
	public static implicit operator int(Rotation rotation) => rotation.Value;
	public static implicit operator Rotation(int value) => new(value);
}
