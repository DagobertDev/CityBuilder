using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Employee))]
public sealed partial class WorkingDecisionSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set(Behavior.Working);
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;
}
