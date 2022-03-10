using System.Numerics;

namespace CityBuilder.Core.Components;

public readonly record struct Size(Vector2 Value)
{
	public Size(float x, float y) : this(new Vector2(x, y)) { }

	public static implicit operator Vector2(Size size) => size.Value;
	public static implicit operator Size(Vector2 size) => new(size);
}
