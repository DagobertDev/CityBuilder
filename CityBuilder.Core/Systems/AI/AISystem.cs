using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class AISystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, in BehaviorQueue behaviorQueue)
	{
		if (behaviorQueue.Count > 0)
		{
			entity.Remove<Idling>();
			behaviorQueue.Dequeue()(entity);
		}
	}

	public static ISystem<float> GetSystems(World world) => new SequentialSystem<float>
	(
		new AISystem(world),
		new EatSystem(world),
		new GoToSleepSystem(world),
		new StartWorkingSystem(world),
		new WanderAroundSystem(world),
		new DoNothingSystem(world)
	);
}
