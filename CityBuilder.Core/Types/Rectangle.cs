using System.Numerics;

namespace CityBuilder.Core.Types;

public readonly record struct Rectangle(Vector2 Position, Vector2 Size)
{
	public bool Intersects(Rectangle other, bool includeBorders = false)
	{
		var (otherPosition, otherSize) = other;

		if (includeBorders)
		{
			if (Position.X > otherPosition.X + otherSize.X || Position.X + Size.X < otherPosition.X ||
				Position.Y > otherPosition.Y + otherSize.Y || Position.Y + Size.Y < otherPosition.Y)
			{
				return false;
			}
		}
		else if (Position.X >= otherPosition.X + otherSize.X || Position.X + Size.X <= otherPosition.X ||
				 Position.Y >= otherPosition.Y + otherSize.Y || Position.Y + Size.Y <= otherPosition.Y)
		{
			return false;
		}

		return true;
	}

	public bool Contains(Vector2 point) => point.X >= Position.X && point.Y >= Position.Y &&
										   point.X < Position.X + Size.X && point.Y < Position.Y + Size.Y;
}
