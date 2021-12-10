using System;
using System.Collections.Generic;
using CityBuilder.Components.Inventory;
using DefaultEcs;

namespace CityBuilder.Systems
{
	public class InventorySystem : IInventorySystem
	{
		private World World { get; }
		private readonly EntityMultiMap<Owner> _inventories;

		public InventorySystem(World world)
		{
			World = world;
			_inventories = World.GetEntities().With<Owner>().AsMultiMap(new SameInventoryComparer());
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
			return entity;
		}

		public Entity? GetGood(Entity owner, string name)
		{
			if (!_inventories.TryGetEntities(new Owner(owner), out var entities))
			{
				entities = ReadOnlySpan<Entity>.Empty;
			}

			var goods = entities.GetEnumerator();

			while (goods.MoveNext())
			{
				var current = goods.Current;
				
				var good = current.Get<Good>();
				
				if (good.Name == name)
				{
					return current;
				}
			}

			return null;
		}
		
		private class SameInventoryComparer : IEqualityComparer<Owner>
		{
			public bool Equals(Owner x, Owner y) => x.Value == y.Value;

			public int GetHashCode(Owner obj) => obj.Value.GetHashCode();
		}
	}
}
