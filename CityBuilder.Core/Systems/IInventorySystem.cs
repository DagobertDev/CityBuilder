using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Core.Systems;

public interface IInventorySystem
{ 
	Entity SetGood(Entity owner, string good, int amount);
	Entity? GetGood(Entity owner, string good);
	ICollection<Entity> GetGoods(Entity owner);
}