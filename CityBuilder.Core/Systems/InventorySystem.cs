using System;
using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;

namespace CityBuilder.Core.Systems;

public class InventorySystem : IInventorySystem
{
	private World World { get; }
	private readonly EntityMap<(Owner, Good)> _ownerAndGood;
	private readonly EntityMultiMap<Owner> _goodsByOwner;

	public InventorySystem(World world)
	{
		World = world;
		_ownerAndGood = World.GetEntities().AsMap<(Owner, Good)>();
		_goodsByOwner = World.GetEntities().AsMultiMap<Owner>();
	}

	public Entity SetGood(Entity owner, string good, int amount)
	{
		if (amount < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(amount));
		}

		if (_ownerAndGood.ContainsKey((new Owner(owner), new Good(good))))
		{
			var existingEntity = GetGood(owner, good);

			existingEntity.Set<Amount>(amount);
			return existingEntity;
		}

		var entity = World.CreateEntity();
		entity.Set(new Owner(owner));
		entity.Set(new Good(good));
		entity.Set<Amount>(amount);
		entity.Set((new Owner(owner), new Good(good)));

		if (owner.Has<Position>())
		{
			entity.SetSameAs<Position>(owner);
		}

		if (owner.Has<Input>() && owner.Get<Input>().Value.ContainsKey(good))
		{
			entity.Set<InventoryPriority>(Priority.High);
		}
		else if (owner.Has<Output>() && owner.Get<Output>().Good == good)
		{
			entity.Set<InventoryPriority>(Priority.Low);
		}
		else
		{
			entity.Set<InventoryPriority>(Priority.Medium);
		}

		return entity;
	}

	public Entity GetGood(Entity owner, string name) =>
		_ownerAndGood[new ValueTuple<Owner, Good>(new Owner(owner), new Good(name))];

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
		if (!_ownerAndGood.ContainsKey((new Owner(owner), new Good(good))))
		{
			SetGood(owner, good, 0);
		}
	}
}
