using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Needs;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(WantsRecreation))]
public sealed partial class WanderAroundDecisionSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set(Behavior.WanderingAround);
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;
}
