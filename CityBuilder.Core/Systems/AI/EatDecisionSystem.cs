using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class EatDecisionSystem : AEntitySetSystem<float>
{
	private const int HungerThreshold = 20;

	private readonly EntitySet _markets;

	public EatDecisionSystem(World world) : base(world, CreateEntityContainer, true)
	{
		_markets = world.GetEntities().With<Market>().With<Position>()
			.With((in Good good) => good.Name == Goods.Food)
			.With((in Amount amount) => amount > 0)
			.AsSet();
	}

	[Update, UseBuffer]
	private void Update(in Entity entity)
	{
		if (_markets.Count == 0)
		{
			return;
		}

		entity.Set<Eating>();
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;

	[WithPredicate]
	private static bool Filter(in Hunger hunger) => hunger >= HungerThreshold;
}
