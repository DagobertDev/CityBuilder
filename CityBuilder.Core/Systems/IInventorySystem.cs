using System.Collections.Generic;
using CityBuilder.Core.Components;
using DefaultEcs;

namespace CityBuilder.Core.Systems;

public interface IInventorySystem
{
	Entity SetGood(Entity owner, string good, int amount);
	Entity GetGood(Entity owner, string good);
	Entity CreatePile(Position position, string good, int amount);
	ICollection<Entity> GetGoods(Entity owner);
	void EnsureCreated(Entity owner, string good);
}
