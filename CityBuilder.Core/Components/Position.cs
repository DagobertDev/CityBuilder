using System.Drawing;
using System.Numerics;

namespace CityBuilder.Core.Components;

public readonly record struct Position(Vector2 Value)
{
	public static implicit operator Vector2(Position position) => position.Value;
	public static implicit operator Position(Vector2 value) => new(value);
}

public static class Vector2Extensions
{
	public static float DistanceTo(this Vector2 one, Vector2 two) => Vector2.Distance(one, two);
	public static float DistanceSquaredTo(this Vector2 one, Vector2 two) => Vector2.DistanceSquared(one, two);
	public static PointF ToPoint(this Vector2 vector) => new(vector.X, vector.Y);

	public static Vector2 MoveToward(this Vector2 from, Vector2 to, float delta)
	{
		var other = to - from;
		var num = other.Length();
		return num <= delta || num < 9.99999997475243E-07 ? to : from + other / num * delta;
	}
}
