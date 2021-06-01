using System;
using CityBuilder.BehaviorTree;
using CityBuilder.BehaviorTree.Nodes;
using CityBuilder.Models;
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
						.GoHome()
						.MoveToRandomLocation()
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
		public static BehaviourTreeBuilder<Entity> GoHome(this BehaviourTreeBuilder<Entity> builder)
		{

			return builder
				.Sequence()
					.Condition(entity => entity.Has<Resident>())
					.Do(entity =>
					{
						entity.Set(new Destination(entity.Get<Resident>().Location));
						return BehaviourTreeStatus.Running;
					})
				.End();
		}


		public static BehaviourTreeBuilder<Entity> MoveToRandomLocation(this BehaviourTreeBuilder<Entity> builder)
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
