using DefaultEcs;

namespace CityBuilder.Messages
{
	public readonly struct EntitySelected
	{
		public EntitySelected(Entity entity)
		{
			Entity = entity;
		}

		public Entity Entity { get; }
	}
}
