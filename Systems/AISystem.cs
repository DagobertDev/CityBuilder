using System;
using CityBuilder.BehaviorTree;
using CityBuilder.BehaviorTree.Nodes;
using CityBuilder.Models;
using CityBuilder.Models.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(Agent))]
	public class AISystem : AEntitySetSystem<float>
	{
		private readonly IBehaviourTreeNode<Entity> _behaviourTree;

		public AISystem(World world) : base(world)
		{
			_behaviourTree =
				new BehaviourTreeBuilder<Entity>()
					.Selector()
						.SatisfyTiredness()
						.GoToWork()
						.GoToRandomLocation()
					.End()
				.Build();
		}

		protected override void Update(float state, in Entity entity)
		{
			_behaviourTree.Tick(entity);
		}
	}

	public static class BehaviorTreeExtensions
	{
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
			return builder.Do(entity => entity.Has<IsAtHome>()
				? BehaviourTreeStatus.Running
				: BehaviourTreeStatus.Failure);
		}

		public static BehaviourTreeBuilder<Entity> GoHomeToSleep(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Get<Tiredness>() >= 20)
					.GoHome()
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoToWork(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Has<Employee>())
					.Do(entity =>
					{
						entity.Set(new Destination(entity.Get<Employee>().Location));
						return BehaviourTreeStatus.Running;
					})
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoHome(this BehaviourTreeBuilder<Entity> builder)
		{
			return builder
				.Sequence()
					.Condition(entity => entity.Has<Resident>())
					.Do(entity =>
					{
						if (entity.Has<IsAtHome>())
						{
							return BehaviourTreeStatus.Running;
						}

						entity.Set(new Destination(entity.Get<Resident>().Location));
						return BehaviourTreeStatus.Running;
					})
				.End();
		}

		public static BehaviourTreeBuilder<Entity> GoToRandomLocation(this BehaviourTreeBuilder<Entity> builder)
		{
			var random = new Random();

			return builder
				.Sequence()
					.Condition(entity => !entity.Has<Destination>())
					.Do(entity =>
						{
							var position = entity.Get<Transform2D>().origin;

							position += new Vector2(500 - 1000 * (float)random.NextDouble(),
								500 - 1000 * (float)random.NextDouble());

							entity.Set(new Destination(position));
							return BehaviourTreeStatus.Success;
						})
				.End();
		}
	}
}
