using System;
using DefaultEcs;

namespace CityBuilder.Core.ModSupport
{
	public class Blueprint
	{
		public Blueprint(string name, Entity entity, Action<Entity> populateEntity)
		{
			Name = name;
			Entity = entity;
			_populateEntity = populateEntity;
		}

		public string Name { get; }
		public Entity Entity { get; }

		private readonly Action<Entity> _populateEntity;

		public void Populate(Entity entity) => _populateEntity(entity);
	}
}
