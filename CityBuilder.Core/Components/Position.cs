using System.Numerics;

namespace CityBuilder.Core.Components;

public readonly record struct Position(Vector2 Value)
{
	public Position(float x, float y) : this(new Vector2(x, y)) { }

	public static implicit operator Vector2(Position position) => position.Value;
	public static implicit operator Position(Vector2 value) => new(value);
}
