using DefaultEcs;

namespace CityBuilder.Components.Inventory
{
	public readonly struct Owner
	{
		public Owner(Entity value)
		{
			Value = value;
		}
		
		public Entity Value { get; }
	}
}
