using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[Without(typeof(Agent))]
[With(typeof(Position))]
public sealed partial class TransportSystem : AEntityMultiMapSystem<float, Good>
{
	private readonly EntityMultiMap<Good> _highPriority;
	private readonly EntityMultiMap<Good> _mediumPriority;

	public TransportSystem(World world) : base(world, true)
	{
		_highPriority = world.GetEntities().With<Position>().Without<Agent>()
			.With((in InventoryPriority value) => value == Priority.High).AsMultiMap<Good>();
		_mediumPriority = world.GetEntities().With<Position>().Without<Agent>()
			.With((in InventoryPriority value) => value == Priority.Medium).AsMultiMap<Good>();
	}

	[Update]
	private void Update(in Good good, in Entity source, in InventoryPriority priority)
	{
		if (_highPriority.TryGetEntities(good, out var highDemand) && !highDemand.IsEmpty)
		{
			var addedAmount = source.Get<Amount>();
			source.Set<Amount>(0);

			var demand = FindBestMarket(source, highDemand);
			demand.Set<Amount>(demand.Get<Amount>() + addedAmount);
		}

		else if (priority == Priority.Low && _mediumPriority.TryGetEntities(good, out var mediumDemand) &&
				 !mediumDemand.IsEmpty)
		{
			var addedAmount = source.Get<Amount>();
			source.Set<Amount>(0);

			var demand = FindBestMarket(source, mediumDemand);
			demand.Set<Amount>(demand.Get<Amount>() + addedAmount);
		}
	}

	[WithPredicate]
	private static bool Filter(in InventoryPriority priority) => priority != Priority.High;

	[WithPredicate]
	private static bool Filter(in Amount amount) => amount > 0;

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
}
