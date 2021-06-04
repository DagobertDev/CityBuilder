using Godot;

namespace CityBuilder.Components
{
	public readonly struct Destination
	{
		public Destination(Vector2 position)
		{
			Position = position;
		}

		public Vector2 Position { get; }
	}
}
