using System;
using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;

namespace CityBuilder.Core.Systems;

public class InventorySystem : IInventorySystem
{
	private const int DefaultInventoryCapacity = 100;
	private World World { get; }

	public InventorySystem(World world)
	{
		World = world;

		World.SubscribeComponentRemoved((in Entity _, in Inventory inventory) =>
		{
			foreach (var good in inventory.Values)
			{
				good.Dispose();
			}
		});
	}

	public Entity SetGood(Entity owner, string good, int amount)
	{
		if (amount < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(amount));
		}

		var inventory = GetGood(owner, good);
		inventory.Set<Amount>(amount);
		return inventory;
	}

	public Entity GetGood(Entity owner, string name) => owner.Get<Inventory>()[name];

	public Entity CreatePile(Position position, string good, int amount)
	{
		var entity = World.CreateEntity();
		entity.Set(new Good(good));
		entity.Set<Amount>(amount);

		entity.Set(position);

		entity.Set<ResourcePile>();
		entity.Set(InventoryType.Supply);
		entity.Set<Capacity>(amount);

		return entity;
	}

	public ICollection<Entity> GetGoods(Entity owner) =>
		owner.Has<Inventory>() ? owner.Get<Inventory>().Values : Array.Empty<Entity>();

	public void EnsureCreated(Entity owner, string good)
	{
		Inventory ownerInventory;

		if (owner.Has<Inventory>())
		{
			ownerInventory = owner.Get<Inventory>();
		}
		else
		{
			ownerInventory = new Inventory(4);
			owner.Set(ownerInventory);
		}

		if (ownerInventory.ContainsKey(good))
		{
			return;
		}

		var entity = World.CreateEntity();
		ownerInventory[good] = entity;
		entity.Set(new Owner(owner));
		entity.Set(new Good(good));
		entity.Set<Amount>(0);
		entity.Set((new Owner(owner), new Good(good)));

		if (owner.Has<Position>())
		{
			entity.SetSameAs<Position>(owner);
		}

		if (owner.Has<Input>() && owner.Get<Input>().Value.ContainsKey(good))
		{
			entity.Set(InventoryType.Demand);

			if (owner.Has<Construction>())
			{
				entity.Set<Capacity>(owner.Get<Input>().Value[good]);
			}
			else
			{
				entity.Set<Capacity>(DefaultInventoryCapacity);
			}
		}
		else if (owner.Has<Output>() && owner.Get<Output>().Good == good || owner.Has<ResourceCollector>())
		{
			entity.Set(InventoryType.Supply);
			entity.Set<Capacity>(DefaultInventoryCapacity);
		}
		else if (owner.Has<Market>())
		{
			entity.Set(InventoryType.Market);
			entity.Set<Capacity>(DefaultInventoryCapacity);
		}
		else
		{
			entity.Set(InventoryType.Manual);
			entity.Set<Capacity>(DefaultInventoryCapacity);
		}
	}
}
