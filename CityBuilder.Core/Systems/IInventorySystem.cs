using DefaultEcs;

namespace CityBuilder.Systems
{
	public interface IInventorySystem
	{ 
		Entity SetGood(Entity owner, string good, int amount);
		Entity? GetGood(Entity owner, string good);
	}
}
