using System;
using System.Numerics;
using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(Idling))]
	public sealed partial class AISystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _markets;

		public AISystem(World world) : base(world, true)
		{
			_markets = world.GetEntities().With<Market>().With<Position>().AsSet();
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
				// SatisfyHunger
				if (entity.Get<Hunger>() >= 20 && _markets.Count > 0)
				{
					var market = FindClosest(entity.Get<Position>().Value, _markets);
					var marketPosition = market.Get<Position>().Value;

					EnqueueBehavior(entity, GoTo(_ => marketPosition), 
						e => e.Set(new Waiting(3)), e =>
					{
						e.Get<Hunger>() = 0;
						e.Set<Idling>();
					});
				}

				// SatisfyTiredness()
				else if (entity.Get<Tiredness>().Value >= 0 && entity.Has<Resident>())
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
						e => { e.Set(new Waiting(5)); });
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
					}), e => e.Set(new Waiting(3)));
				}
			}
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
}
