using System;
using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Components
{
	public readonly struct Resident
	{
		public Resident(Entity home)
		{
			_home = home;
			Location = home.Get<Position>().Value;
		}

		private readonly Entity _home;
		public Entity Home => _home.IsAlive ? _home : throw new ApplicationException("Entity is not alive");
		public Vector2 Location { get; }
	}
}
