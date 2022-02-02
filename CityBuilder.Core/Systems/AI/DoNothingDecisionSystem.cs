using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class DoNothingDecisionSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set(Behavior.DoingNothing);
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;
}
