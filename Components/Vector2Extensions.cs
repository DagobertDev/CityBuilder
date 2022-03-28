using Godot;

namespace CityBuilder.Components;

public static class Vector2Extensions
{
	public static Vector2 ToGodotVector(this System.Numerics.Vector2 vector) => new(vector.X, vector.Y);
	public static System.Numerics.Vector2 ToNumericsVector(this Vector2 vector) => new(vector.x, vector.y);
}
