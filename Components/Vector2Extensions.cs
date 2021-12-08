using System.Numerics;

namespace CityBuilder.Components
{
	public static class Vector2Extensions
	{
		public static Godot.Vector2 ToGodotVector(this Vector2 vector) => new(vector.X, vector.Y);
		public static Vector2 ToNumericsVector(this Godot.Vector2 vector) => new(vector.x, vector.y);
	}
}
