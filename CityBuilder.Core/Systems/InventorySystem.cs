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
	private readonly EntityMap<(Owner, Good)> _ownerAndGood;
	private readonly EntityMultiMap<Owner> _goodsByOwner;

	public InventorySystem(World world)
	{
		World = world;
		_ownerAndGood = World.GetEntities().AsMap<(Owner, Good)>();
		_goodsByOwner = World.GetEntities().AsMultiMap<Owner>();

		world.SubscribeEntityDisposed((in Entity entity) =>
		{
			foreach (var inventory in GetGoods(entity))
			{
				inventory.Dispose();
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

	public Entity GetGood(Entity owner, string name) =>
		_ownerAndGood[(new Owner(owner), new Good(name))];

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

	public ICollection<Entity> GetGoods(Entity owner)
	{
		if (_goodsByOwner.TryGetEntities(new Owner(owner), out var entities))
		{
			return entities.ToArray();
		}

		return Array.Empty<Entity>();
	}

	public void EnsureCreated(Entity owner, string good)
	{
		if (_ownerAndGood.ContainsKey((new Owner(owner), new Good(good))))
		{
			return;
		}

		var entity = World.CreateEntity();
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
