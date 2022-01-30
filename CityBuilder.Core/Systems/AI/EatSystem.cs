using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class EatSystem : AEntitySetSystem<float>
{
	private const int HungerThreshold = 20;
	private const int FoodHungerReduction = 10;

	private readonly EntitySet _markets;

	public EatSystem(World world) : base(world, CreateEntityContainer, true)
	{
		_markets = world.GetEntities().With<Market>().With<Position>()
			.With((in Good good) => good.Name == Goods.Food)
			.With((in Amount amount) => amount > 0)
			.AsSet();
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, in Position position, in BehaviorQueue behaviorQueue)
	{
		if (_markets.Count == 0)
		{
			return;
		}

		var market = FindBestMarket(position);
		var marketPosition = market.Get<Position>().Value;

		entity.Set<Destination>(marketPosition);

		behaviorQueue.Enqueue(e => Eat(e, market));
	}

	[WithPredicate]
	private static bool Filter(in Hunger hunger) => hunger >= HungerThreshold;

	private static void Eat(Entity entity, Entity market)
	{
		var hunger = entity.Get<Hunger>().Value;
		var availableFood = market.Get<Amount>().Value;
		var eatenFood = Math.Min((int)hunger / FoodHungerReduction, availableFood);

		market.Set<Amount>(availableFood - eatenFood);
		entity.Set<Hunger>(hunger - eatenFood * FoodHungerReduction);
		entity.Set<Waiting>(eatenFood);
	}

	private Entity FindBestMarket(Vector2 position)
	{
		var distance = float.MaxValue;
		var closest = default(Entity);

		foreach (var market in _markets.GetEntities())
		{
			var newDistance = market.Get<Position>().Value.DistanceSquaredTo(position);

			if (newDistance < distance)
			{
				distance = newDistance;
				closest = market;
			}
		}

		if (closest.IsAlive == false)
		{
			throw new ApplicationException("Market not found");
		}

		return closest;
	}
}
