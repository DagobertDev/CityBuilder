using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class DoNothingStateSystem : AEntitySetSystem<float>
{
	private const int WaitDuration = 5;

	[Update, UseBuffer]
	private static void Update(in Entity entity, [Changed] in BehaviorState state)
	{
		switch (state)
		{
			case Starting:
				entity.Set<Waiting>(WaitDuration);
				break;
			case Finished:
				entity.Remove<Behavior>();
				entity.Set(BehaviorState.Deciding);
				break;
		}
	}

	[WithPredicate]
	private static bool Filter(in Behavior behavior) => behavior == Behavior.DoingNothing;

	private const int Starting = BehaviorState.StartingValue;
	private const int Finished = Starting + 1;
}
