using DefaultEcs;
using Godot;

namespace CityBuilder.Components
{
	public readonly struct Employee
	{
		public Employee(Entity workplace)
		{
			Workplace = workplace;
			Location = workplace.Get<Transform2D>().origin;
		}

		public Entity Workplace { get; }
		public Vector2 Location { get; }
	}
}
