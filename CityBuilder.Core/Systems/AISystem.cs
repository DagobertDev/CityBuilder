using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Idling))]
public sealed partial class AISystem : AEntitySetSystem<float>
{
	private readonly EntitySet _markets;

	public AISystem(World world) : base(world, true)
	{
		_markets = world.GetEntities().With<Market>().With<Position>()
			.With((in Good good) => good.Name == Goods.Food)
			.With((in Amount amount) => amount >= 1)
			.AsSet();
	}

	[Update]
	private void Update(in Entity entity, in BehaviorQueue behaviorQueue)
	{
		if (behaviorQueue.Count > 0)
		{
			entity.Remove<Idling>();
			behaviorQueue.Dequeue()(entity);
		}
		else
		{
			switch (entity.Get<Agent>().Type)
			{
				case AIType.Worker:
					WorkerAI(entity);
					break;
				case AIType.Transporter:
					TransporterAI(entity);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void WorkerAI(in Entity entity)
	{
		// SatisfyHunger
		if (entity.Get<Hunger>() >= 20 && _markets.Count > 0)
		{
			var market = FindClosest(entity.Get<Position>().Value, _markets);
			var marketPosition = market.Get<Position>().Value;

			EnqueueBehavior(entity, GoTo(_ => marketPosition), e =>
			{
				const int foodHungerReduction = 10;

				var hunger = e.Get<Hunger>().Value;
				var availableFood = market.Get<Amount>().Value;
				var eatenFood = Math.Min((int)hunger / foodHungerReduction, availableFood);

				market.Set<Amount>(availableFood - eatenFood);
				e.Set(new Hunger(hunger - eatenFood * foodHungerReduction));
				e.Set<Waiting>(eatenFood);
			});
		}

		// SatisfyTiredness()
		else if (entity.Get<Tiredness>() >= 0 && entity.Has<Resident>())
		{
			if (entity.Has<IsAtHome>())
			{
				EnqueueBehavior(entity, e => e.Set<Sleeping>());
			}

			else if (entity.Get<Tiredness>() >= 20)
			{
				EnqueueBehavior(entity, GoTo(e => e.Get<Resident>().Location), e => e.Set<Sleeping>());
			}
		}

		// GoToWork
		else if (entity.Has<Employee>())
		{
			EnqueueBehavior(entity, e => { e.Set(new Destination(e.Get<Employee>().Location)); },
				e => { e.Set<Waiting>(5); });
		}

		// GoToRandomLocation
		else
		{
			var random = new Random();

			EnqueueBehavior(entity, GoTo(e =>
			{
				var position = e.Get<Position>().Value;
				position += new Vector2(500 - 1000 * (float)random.NextDouble(),
					500 - 1000 * (float)random.NextDouble());
				return position;
			}), e => e.Set<Waiting>(3));
		}
	}

	private void TransporterAI(in Entity entity)
	{
		if (!entity.Has<Transport>())
		{
			EnqueueBehavior(entity, e => e.Set<Waiting>(5));
			return;
		}

		// Go to start
		EnqueueBehavior(entity, GoTo(e =>
		{
			var from = e.Get<Transport>().From;

			if (!from.IsAlive)
			{
				throw new ApplicationException("Transport starting point is not alive.");
			}

			return from.Get<Position>().Value;
		}));

		// Pick goods
		EnqueueBehavior(entity, e =>
		{
			var transport = e.Get<Transport>();
			var from = transport.From;

			if (!from.IsAlive)
			{
				throw new ApplicationException("Transport start point is not alive.");
			}

			var requestedAmount = transport.Amount;
			var availableAmount = from.Get<Amount>();
			var transportedAmount = Math.Min(requestedAmount, availableAmount);

			from.Set<Amount>(availableAmount - transportedAmount);
			e.Set(transport with { Amount = transportedAmount, Delivering = true });

			e.Set<Waiting>(1);
		});

		// Go to end
		EnqueueBehavior(entity, GoTo(e =>
		{
			var transport = e.Get<Transport>();
			var to = transport.To;

			if (!to.IsAlive)
			{
				throw new ApplicationException("Transport end point is not alive.");
			}

			return to.Get<Position>().Value;
		}));

		// Drop goods
		EnqueueBehavior(entity, e =>
		{
			var transport = e.Get<Transport>();
			var to = transport.To;

			if (!to.IsAlive)
			{
				throw new ApplicationException("Transport end point is not alive.");
			}

			var addedAmount = transport.Amount;
			to.Get<Amount>() += addedAmount;
			to.NotifyChanged<Amount>();

			e.Remove<Transport>();
			e.Set<Waiting>(1);
		});
	}

	private static Entity FindClosest(Vector2 position, EntitySet entities)
	{
		var distance = float.MaxValue;
		var closest = default(Entity);

		foreach (var market in entities.GetEntities())
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

	private static Action<Entity> GoTo(Func<Entity, Vector2> getLocation) =>
		entity => entity.Set(new Destination(getLocation(entity)));

	private static void EnqueueBehavior(Entity entity, params Action<Entity>[] actions)
	{
		var queue = entity.Get<BehaviorQueue>();

		foreach (var action in actions)
		{
			queue.Enqueue(action);
		}
	}
}
