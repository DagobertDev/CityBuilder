using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Components
{
	public readonly struct Employee
	{
		public Employee(Entity workplace)
		{
			Workplace = workplace;
			Location = workplace.Get<Position>().Value;
		}

		public Entity Workplace { get; }
		public Vector2 Location { get; }
	}
}
