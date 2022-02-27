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
		var toMap = world.GetEntities().AsMultiMap(new ToComparer());

		world.SubscribeComponentAdded((in Entity entity, in Amount _) => UpdateCapacity(entity));
		world.SubscribeComponentChanged((in Entity entity, in Amount _, in Amount _) => UpdateCapacity(entity));

		world.SubscribeComponentAdded((in Entity entity, in Capacity _) => UpdateCapacity(entity));
		world.SubscribeComponentChanged((in Entity entity, in Capacity _, in Capacity _) => UpdateCapacity(entity));

		void UpdateCapacity(in Entity entity)
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

		world.SubscribeComponentAdded((in Entity _, in Transport transport) =>
			UpdateFutureUnusedCapacity(transport.To));
		world.SubscribeComponentChanged((in Entity _, in Transport transport, in Transport _) =>
			UpdateFutureUnusedCapacity(transport.To));
		world.SubscribeComponentRemoved((in Entity _, in Transport transport) =>
			UpdateFutureUnusedCapacity(transport.To));

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

	private class ToComparer : IEqualityComparer<Transport>
	{
		public bool Equals(Transport x, Transport y) => x.To == y.To;
		public int GetHashCode(Transport obj) => obj.To.GetHashCode();
	}

	public void Dispose() { }

	public void Update(float state) { }

	public bool IsEnabled { get; set; } = true;
}
