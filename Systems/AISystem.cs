using System;
using CityBuilder.BehaviorTree;
using CityBuilder.BehaviorTree.Nodes;
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
			var random = new Random();

			_behaviourTree = new BehaviourTreeBuilder<Entity>()
					.SetRandomDestination()
				.Build();
		}

		protected override void Update(float state, in Entity entity)
		{
			_behaviourTree.Tick(entity);
		}
	}

	public static class BehaviorTreeExtensions
	{
		public static BehaviourTreeBuilder<Entity> SetRandomDestination(this BehaviourTreeBuilder<Entity> builder)
		{
			var random = new Random();

			return builder
				.Sequence()
					.Condition(data => !data.Has<Destination>())
					.Do(data =>
						{
							var position = new Vector2(10000 * (float)random.NextDouble(), 10000 * (float)random.NextDouble());

							data.Set(new Destination(position));
							return BehaviourTreeStatus.Success;
						})
				.End();
		}
	}
}
