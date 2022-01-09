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
	private readonly EntitySet _transporters;

	public TransportSystem(World world) : base(world, true)
	{
		_highPriority = world.GetEntities().With<Position>().Without<Agent>()
			.With((in InventoryPriority value) => value == Priority.High).AsMultiMap<Good>();
		_mediumPriority = world.GetEntities().With<Position>().Without<Agent>()
			.With((in InventoryPriority value) => value == Priority.Medium).AsMultiMap<Good>();
		_transporters = world.GetEntities().With((in Agent agent) => agent.Type == AIType.Transporter)
			.Without<Transport>().AsSet();
	}

	[Update]
	private void Update(in Good good, in Entity source, in InventoryPriority priority)
	{
		if (_transporters.Count == 0)
		{
			return;
		}

		if (_highPriority.TryGetEntities(good, out var highDemand) && !highDemand.IsEmpty)
		{
			var demand = FindBestMarket(source, highDemand);
			var transporter = FindBestTransporter(source, _transporters.GetEntities());
			transporter.Set(new Transport(source, demand, good, int.MaxValue));
		}

		else if (priority == Priority.Low && _mediumPriority.TryGetEntities(good, out var mediumDemand) &&
				 !mediumDemand.IsEmpty)
		{
			var demand = FindBestMarket(source, mediumDemand);
			var transporter = FindBestTransporter(source, _transporters.GetEntities());
			transporter.Set(new Transport(source, demand, good, int.MaxValue));
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
