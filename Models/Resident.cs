using DefaultEcs;
using Godot;

namespace CityBuilder.Models
{
	public readonly struct Resident
	{
		public Resident(Entity home)
		{
			Home = home;
			Location = home.Get<Transform2D>().origin;
		}

		public Entity Home { get; }
		public Vector2 Location { get; }
	}
}
