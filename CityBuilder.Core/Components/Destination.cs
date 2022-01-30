using System.Numerics;

namespace CityBuilder.Core.Components;

public readonly record struct Destination(Vector2 Position)
{
	public static implicit operator Vector2(Destination destination) => destination.Position;
	public static implicit operator Destination(Vector2 position) => new(position);
}
