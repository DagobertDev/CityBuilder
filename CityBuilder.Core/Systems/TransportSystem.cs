using CityBuilder.Components;
using CityBuilder.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
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
				var addedAmount = source.Get<Amount>().Value;
				source.Set(new Amount(0));

				var market = _markets[good][0];
				var currentAmount = market.Get<Amount>().Value;
				market.Set(new Amount(currentAmount + addedAmount));
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			_markets.Dispose();
		}
	}
}
