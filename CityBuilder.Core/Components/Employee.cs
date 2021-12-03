using System;
using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Components
{
	public readonly struct Employee
	{
		public Employee(Entity workplace)
		{
			_workplace = workplace;
			Location = workplace.Get<Position>().Value;
		}

		private readonly Entity _workplace;
		public Entity Workplace => _workplace.IsAlive ? _workplace : throw new ApplicationException("Entity is not alive");
		public Vector2 Location { get; }
	}
}
