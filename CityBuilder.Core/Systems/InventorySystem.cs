using System;
using System.Collections.Generic;
using CityBuilder.Components.Inventory;
using DefaultEcs;

namespace CityBuilder.Systems
{
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
			
			var nullableEntity = GetGood(owner, good);

			if (nullableEntity.HasValue)
			{
				nullableEntity.Value.Set(new Amount(amount));
				return nullableEntity.Value;
			}
			
			var entity = World.CreateEntity();
			entity.Set(new Owner(owner));
			entity.Set(new Good(good));
			entity.Set(new Amount(amount));
			entity.Set((new Owner(owner), new Good(good)));
			return entity;
		}

		public Entity? GetGood(Entity owner, string name)
		{
			if (_ownerAndGood.TryGetEntity(new (new Owner(owner), new Good(name)), out var entity))
			{
				return entity;
			}

			return null;
		}
		
		public ICollection<Entity> GetGoods(Entity owner)
		{
			if (_goodsByOwner.TryGetEntities(new Owner(owner), out var entities))
			{
				return entities.ToArray();
			}

			return Array.Empty<Entity>();
		}
	}
}
