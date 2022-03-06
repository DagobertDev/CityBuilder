using System.Numerics;

namespace CityBuilder.Core.Components;

public readonly record struct Waypoint(Vector2 Position)
{
	public static implicit operator Vector2(Waypoint waypoint) => waypoint.Position;
	public static implicit operator Waypoint(Vector2 position) => new(position);
}
