using System.Numerics;

namespace CityBuilder.Components
{
	public readonly struct Position
	{
		public Position(Vector2 value)
		{
			Value = value;
		}

		public Vector2 Value { get; }
	}

	public static class Vector2Extensions
	{
		public static float DistanceTo(this Vector2 one, Vector2 two) => Vector2.Distance(one, two);
		public static float DistanceSquaredTo(this Vector2 one, Vector2 two) => Vector2.DistanceSquared(one, two);

		public static Vector2 MoveToward(this Vector2 from, Vector2 to, float delta)
		{
			var other = to - from;
			var num = other.Length();
			return num <= delta || num < 9.99999997475243E-07 ? to : from + other / num * delta;
		}
		public static Godot.Vector2 ToGodotVector(this Vector2 vector) => new(vector.X, vector.Y);
		public static Vector2 ToNumericsVector(this Godot.Vector2 vector) => new(vector.x, vector.y);
	}
}
