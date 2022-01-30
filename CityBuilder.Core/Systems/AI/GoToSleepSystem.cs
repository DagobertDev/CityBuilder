using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class GoToSleepSystem : AEntitySetSystem<float>
{
	private const int Threshold = 20;

	[Update, UseBuffer]
	private static void Update(in Entity entity, in Resident resident, in BehaviorQueue behaviorQueue)
	{
		entity.Set<Destination>(resident.Location);
		behaviorQueue.Enqueue(Sleep);
	}

	[WithPredicate]
	private static bool Filter(in Tiredness tiredness) => tiredness >= Threshold;

	private static void Sleep(Entity entity) => entity.Set<Sleeping>();
}
