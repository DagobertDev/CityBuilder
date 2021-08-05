using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Components
{
	public readonly struct Resident
	{
		public Resident(Entity home)
		{
			Home = home;
			Location = home.Get<Position>().Value;
		}

		public Entity Home { get; }
		public Vector2 Location { get; }
	}
}
