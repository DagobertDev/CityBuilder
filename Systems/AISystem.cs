using System;
using CityBuilder.BehaviorTree;
using CityBuilder.BehaviorTree.Nodes;
using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(Idling))]
	public sealed partial class AISystem : AEntitySetSystem<float>
	{
		private readonly IBehaviourTreeNode<Entity> _behaviourTree;

		public AISystem(World world) : base(world, true)
		{
			var markets = world.GetEntities().With<Market>().With<Transform2D>().AsSet();

			_behaviourTree =
				new BehaviourTreeBuilder<Entity>()
					.Selector()
						.SatisfyHunger(markets)
						.SatisfyTiredness()
						.GoToWork()
						.GoToRandomLocation()
					.End()
				.Build();
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
				_behaviourTree.Tick(entity);
			}
		}
	}

	public static class BehaviorTreeExtensions
	{
		public static BehaviourTreeBuilder<Entity> SatisfyHunger(this BehaviourTreeBuilder<Entity> builder, EntitySet markets)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Get<Hunger>().Value >= 20 && markets.Count > 0)
					.EnqueueBehavior(GoTo(entity =>
					{
						var position = entity.Get<Transform2D>().origin;
						var distance = float.MaxValue;
						var closest = default(Entity);

						foreach (var market in markets.GetEntities())
						{
							var newDistance = market.Get<Transform2D>().origin.DistanceSquaredTo(position);
							
							if (newDistance < distance)
							{
								distance = newDistance;
								closest = market;
							}
						}

						if (closest == default)
						{
							throw new ApplicationException("Market not found");
						}

						return closest.Get<Transform2D>().origin;
					}), entity => entity.Set(new Waiting(3)), 
					entity =>
					{
						entity.Get<Hunger>() = 0;
						entity.Set<Idling>();
					})
				.End();
		}

		public static BehaviourTreeBuilder<Entity> SatisfyTiredness(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Get<Tiredness>() >= 0)
					.Selector()
						.Sleep()
						.GoHomeToSleep()
					.End()
				.End();
		}

		public static BehaviourTreeBuilder<Entity> Sleep(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Has<IsAtHome>())
					.EnqueueBehavior(entity => entity.Set<Sleeping>())
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoHomeToSleep(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Get<Tiredness>() >= 20)
					.Condition(entity => entity.Has<Resident>())
					.EnqueueBehavior(GoTo(entity => entity.Get<Resident>().Location), 
						entity => entity.Set<Sleeping>())
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoToWork(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Has<Employee>())
					.EnqueueBehavior(entity =>
					{
						entity.Set(new Destination(entity.Get<Employee>().Location));
					}, entity =>
					{
						entity.Set(new Waiting(5));
					})
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoToRandomLocation(this BehaviourTreeBuilder<Entity> builder)
		{
			var random = new Random();

			return builder
				.EnqueueBehavior(GoTo(entity => 
				{ 
					var position = entity.Get<Transform2D>().origin; 
					position += new Vector2(500 - 1000 * (float)random.NextDouble(), 
						500 - 1000 * (float)random.NextDouble()); 
					return position; 
				}), entity => entity.Set(new Waiting(3)));
		}

		private static Action<Entity> GoTo(Func<Entity, Vector2> getLocation) => entity => entity.Set(new Destination(getLocation(entity)));

		public static BehaviourTreeBuilder<Entity> EnqueueBehavior(this BehaviourTreeBuilder<Entity> builder, params Action<Entity>[] actions)
		{
			return builder.Do(entity =>
			{
				var queue = entity.Get<BehaviorQueue>();
				
				foreach (var action in actions)
				{
					queue.Enqueue(action);
				}
				
				return BehaviourTreeStatus.Running;
			});
		}
	}
}
