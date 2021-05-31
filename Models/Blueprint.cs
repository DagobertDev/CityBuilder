using DefaultEcs;

namespace CityBuilder.Models
{
	public class Blueprint
	{
		public Blueprint(string name, Entity entity)
		{
			Name = name;
			Entity = entity;
		}

		public string Name { get; }
		public Entity Entity { get; }
	}
}
