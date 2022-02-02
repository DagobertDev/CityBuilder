using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class SleepStateSystem : AEntitySetSystem<float>
{
	private const int Threshold = 20;

	[Update, UseBuffer]
	private static void Update(in Entity entity, [Changed] in BehaviorState state)
	{
		switch (state)
		{
			case Starting:
				GoToSleep(entity);
				break;
			case Arrived:
				Sleep(entity);
				break;
			case Finished:
				OnFinished(entity);
				break;
		}
	}

	[WithPredicate]
	private static bool Filter(in Behavior behavior) => behavior == Behavior.Sleeping;

	private static void GoToSleep(Entity entity)
	{
		var resident = entity.Get<Resident>();
		entity.Set<Destination>(resident.Location);
	}

	private static void Sleep(Entity entity)
	{
		entity.Set<Sleeping>();
	}

	private static void OnFinished(Entity entity)
	{
		entity.Remove<Behavior>();
		entity.Set(BehaviorState.Deciding);
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Arrived = Starting + 1;
	private const int Finished = Arrived + 1;
}
