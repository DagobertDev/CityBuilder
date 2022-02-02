using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class WorkingStateSystem : AEntitySetSystem<float>
{
	private const float WorkTime = 3;

	[Update, UseBuffer]
	private static void Update(in Entity entity, [Changed] in BehaviorState state)
	{
		switch (state)
		{
			case Starting:
				GoToWork(entity);
				break;
			case Arrived:
				Work(entity);
				break;
			case Finished:
				OnFinished(entity);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(state));
		}
	}

	[WithPredicate]
	private static bool Filter(in Behavior behavior) => behavior == Behavior.Working;

	private static void GoToWork(Entity entity)
	{
		var employee = entity.Get<Employee>();
		entity.Set<Destination>(employee.Location);
	}

	private static void Work(Entity entity)
	{
		entity.Set<Waiting>(WorkTime);
	}

	private static void OnFinished(Entity entity)
	{
		entity.Set(BehaviorState.Deciding);
		entity.Remove<Behavior>();
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Arrived = Starting + 1;
	private const int Finished = Arrived + 1;
}
