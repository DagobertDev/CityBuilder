using System.Collections.Generic;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed class TransportReservationSystem : ISystem<float>
{
	public TransportReservationSystem(World world)
	{
		var fromMap = world.GetEntities().AsMultiMap(new FromComparer());
		var toMap = world.GetEntities().AsMultiMap(new ToComparer());

		world.SubscribeComponentAdded((in Entity entity, in Amount _) => UpdateFutureAmount(entity));
		world.SubscribeComponentChanged((in Entity entity, in Amount _, in Amount _) => UpdateFutureAmount(entity));

		world.SubscribeComponentAdded((in Entity _, in Transport transport) =>
			UpdateAmountAndCapacity(transport));
		world.SubscribeComponentChanged((in Entity _, in Transport transport, in Transport _) =>
			UpdateAmountAndCapacity(transport));
		world.SubscribeComponentRemoved((in Entity _, in Transport transport) =>
			UpdateAmountAndCapacity(transport));

		void UpdateAmountAndCapacity(Transport transport)
		{
			UpdateFutureAmount(transport.From);
			UpdateFutureUnusedCapacity(transport.To);
		}

		void UpdateFutureAmount(in Entity inventory)
		{
			if (!inventory.IsAlive)
			{
				return;
			}

			var amount = inventory.Get<Amount>();

			if (!fromMap.TryGetEntities(new Transport(inventory, default, default, default), out var transports))
			{
				inventory.Set<FutureAmount>((int)amount);
				return;
			}

			var removedAmount = 0;

			foreach (var t in transports)
			{
				removedAmount += t.Get<Transport>().Amount;
			}

			inventory.Set<FutureAmount>(amount - removedAmount);
		}

		world.SubscribeComponentAdded((in Entity entity, in Amount _) => UpdateUnusedCapacity(entity));
		world.SubscribeComponentChanged((in Entity entity, in Amount _, in Amount _) => UpdateUnusedCapacity(entity));

		world.SubscribeComponentAdded((in Entity entity, in Capacity _) => UpdateUnusedCapacity(entity));
		world.SubscribeComponentChanged(
			(in Entity entity, in Capacity _, in Capacity _) => UpdateUnusedCapacity(entity));

		void UpdateUnusedCapacity(in Entity entity)
		{
			if (entity.Has<Capacity>())
			{
				entity.Set<UnusedCapacity>(entity.Get<Capacity>() - entity.Get<Amount>());
			}
			else
			{
				entity.Set<UnusedCapacity>(int.MaxValue);
			}

			UpdateFutureUnusedCapacity(entity);
		}

		void UpdateFutureUnusedCapacity(in Entity to)
		{
			if (!to.IsAlive)
			{
				return;
			}

			var unusedCapacity = to.Get<UnusedCapacity>();

			if (!toMap.TryGetEntities(new Transport(default, to, default, default), out var transports))
			{
				to.Set<FutureUnusedCapacity>(unusedCapacity.Value);
				return;
			}

			var addedAmount = 0;

			foreach (var t in transports)
			{
				addedAmount += t.Get<Transport>().Amount;
			}

			to.Set<FutureUnusedCapacity>(unusedCapacity - addedAmount);
		}
	}

	private class FromComparer : IEqualityComparer<Transport>
	{
		public bool Equals(Transport x, Transport y) => x.From == y.From;
		public int GetHashCode(Transport obj) => obj.From.GetHashCode();
	}

	private class ToComparer : IEqualityComparer<Transport>
	{
		public bool Equals(Transport x, Transport y) => x.To == y.To;
		public int GetHashCode(Transport obj) => obj.To.GetHashCode();
	}

	public void Dispose() { }

	public void Update(float state) { }

	public bool IsEnabled { get; set; } = true;
}
