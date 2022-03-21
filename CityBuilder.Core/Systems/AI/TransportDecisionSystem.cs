using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[Without(typeof(Agent))]
[With(typeof(Position))]
public sealed partial class TransportDecisionSystem : AEntityMultiMapSystem<float, Good>
{
	private readonly EntityMultiMap<Good> _highPriority;
	private readonly EntityMultiMap<Good> _mediumPriority;
	private readonly EntitySet _transporters;

	public TransportDecisionSystem(World world) : base(world, true)
	{
		_highPriority = world.GetEntities().With<Position>()
			.With((in InventoryType value) => value == InventoryType.Demand)
			.With((in FutureUnusedCapacity capacity) => capacity > 0)
			.AsMultiMap<Good>();
		_mediumPriority = world.GetEntities().With<Position>()
			.With((in InventoryType value) => value == InventoryType.Market)
			.With((in FutureUnusedCapacity capacity) => capacity > 0)
			.AsMultiMap<Good>();
		_transporters = world.GetEntities().With<TransportCapacity>()
			.With((in BehaviorState state) => state.HasNotDecided)
			.Without<Transport>().AsSet();
	}

	[Update]
	private void Update(in Good good, in Entity source, in InventoryType type)
	{
		if (_transporters.Count == 0)
		{
			return;
		}

		if (_highPriority.TryGetEntities(good, out var highDemand) && !highDemand.IsEmpty)
		{
			StartTransport(source, good, highDemand);
		}

		else if (type == InventoryType.Supply && _mediumPriority.TryGetEntities(good, out var mediumDemand) &&
				 !mediumDemand.IsEmpty)
		{
			StartTransport(source, good, mediumDemand);
		}

		void StartTransport(in Entity src, in Good gd, in ReadOnlySpan<Entity> demanders)
		{
			var demand = FindBestMarket(src, demanders);
			var transporter = FindBestTransporter(src, _transporters.GetEntities());
			var capacity = transporter.Get<TransportCapacity>().Value;
			var transportedAmount = Math.Min(capacity, demand.Get<FutureUnusedCapacity>());
			transporter.Set(new Transport(src, demand, gd, transportedAmount));
			transporter.Set(BehaviorState.Starting);
		}
	}

	[WithPredicate]
	private static bool Filter(in InventoryType type) => type is InventoryType.Supply or InventoryType.Market;

	[WithPredicate]
	private static bool Filter(in FutureAmount amount) => amount > 0;

	public override void Dispose()
	{
		base.Dispose();
		_highPriority.Dispose();
		_mediumPriority.Dispose();
	}

	private static Entity FindBestMarket(Entity source, ReadOnlySpan<Entity> markets)
	{
		var position = source.Get<Position>().Value;
		Entity bestMatch = default;
		var currentDistance = float.MaxValue;

		foreach (var market in markets)
		{
			var marketPosition = market.Get<Position>().Value;
			var distance = position.DistanceSquaredTo(marketPosition);

			if (distance < currentDistance)
			{
				bestMatch = market;
				currentDistance = distance;
			}
		}

		return bestMatch;
	}

	private static Entity FindBestTransporter(Entity source, ReadOnlySpan<Entity> transporters)
	{
		var position = source.Get<Position>().Value;
		Entity bestMatch = default;
		var currentDistance = float.MaxValue;

		foreach (var transporter in transporters)
		{
			var transporterPosition = transporter.Get<Position>().Value;
			var distance = position.DistanceSquaredTo(transporterPosition);

			if (distance < currentDistance)
			{
				bestMatch = transporter;
				currentDistance = distance;
			}
		}

		return bestMatch;
	}
}
