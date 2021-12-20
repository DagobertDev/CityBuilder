using System;
using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Core.Components
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

		public override bool Equals(object other) => other is Resident resident && _home == resident._home;
		public override int GetHashCode() => _home.GetHashCode();
	}
}
