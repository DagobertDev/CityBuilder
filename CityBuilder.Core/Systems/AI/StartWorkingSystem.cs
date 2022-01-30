using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class StartWorkingSystem : AEntitySetSystem<float>
{
	private const float WorkTime = 3;

	[Update, UseBuffer]
	private static void Update(in Entity entity, in Employee employee, in BehaviorQueue behaviorQueue)
	{
		entity.Set<Destination>(employee.Location);
		behaviorQueue.Enqueue(Work);
	}

	private static void Work(Entity entity) => entity.Set<Waiting>(WorkTime);
}
