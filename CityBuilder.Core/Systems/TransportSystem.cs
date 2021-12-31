using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[Without(typeof(Market))]
public sealed class TransportSystem : AEntityMultiMapSystem<float, Good>
{
	private readonly EntityMultiMap<Good> _markets;

	public TransportSystem(World world) : base(world, true)
	{
		_markets = world.GetEntities().With<Market>().AsMultiMap<Good>();
	}

	protected override void Update(float state, in Good good, in Entity source)
	{
		if (_markets.TryGetEntities(good, out var markets) && !markets.IsEmpty)
		{
			var addedAmount = source.Get<Amount>();
			source.Set<Amount>(0);

			var market = FindBestMarket(source, markets);
			market.Set<Amount>(market.Get<Amount>() + addedAmount);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		_markets.Dispose();
	}

	private static Entity FindBestMarket(Entity source, ReadOnlySpan<Entity> markets)
	{
		var position = source.Get<Position>().Value;
		Entity bestMatch = default;
		var currentDistance = float.MaxValue;

		foreach (var market in markets)
		{
			var workplacePosition = market.Get<Position>().Value;
			var distance = position.DistanceSquaredTo(workplacePosition);

			if (distance < currentDistance)
			{
				bestMatch = market;
				currentDistance = distance;
			}
		}

		return bestMatch;
	}
}
