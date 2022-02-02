using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class EatStateSystem : AEntitySetSystem<float>
{
	private const int FoodHungerReduction = 10;

	private readonly EntitySet _markets;

	public EatStateSystem(World world) : base(world, CreateEntityContainer, true)
	{
		_markets = world.GetEntities().With<Market>().With<Position>()
			.With((in Good good) => good.Name == Goods.Food)
			.With((in Amount amount) => amount > 0)
			.AsSet();
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, in BehaviorState state, ref Eating eating)
	{
		switch (state)
		{
			case Starting:
				GoToFoodSource(entity, ref eating);
				break;
			case Arrived:
				Eat(entity, in eating);
				break;
			case Finished:
				OnFinished(entity);
				break;
		}
	}

	private void GoToFoodSource(Entity entity, ref Eating eating)
	{
		var position = entity.Get<Position>();
		var market = FindBestMarket(position);
		eating = eating with { Market = market };

		var marketPosition = market.Get<Position>().Value;
		entity.Set<Destination>(marketPosition);
	}

	private static void Eat(Entity entity, in Eating eating)
	{
		var market = eating.Market ?? throw new ApplicationException("Market does no longer exist");
		var hunger = entity.Get<Hunger>().Value;
		var availableFood = market.Get<Amount>().Value;
		var eatenFood = Math.Min((int)hunger / FoodHungerReduction, availableFood);

		market.Set<Amount>(availableFood - eatenFood);
		entity.Set<Hunger>(hunger - eatenFood * FoodHungerReduction);
		entity.Set<Waiting>(eatenFood);
	}

	private static void OnFinished(Entity entity)
	{
		entity.Remove<Eating>();
		entity.Set(BehaviorState.Deciding);
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

	private const int Starting = BehaviorState.StartingValue;
	private const int Arrived = Starting + 1;
	private const int Finished = Arrived + 1;
}
