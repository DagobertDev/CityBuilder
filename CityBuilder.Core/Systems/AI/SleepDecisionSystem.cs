using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Resident))]
public sealed partial class SleepDecisionSystem : AEntitySetSystem<float>
{
	private const int Threshold = 20;

	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set(Behavior.Sleeping);
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;

	[WithPredicate]
	private static bool Filter(in Tiredness tiredness) => tiredness >= Threshold;
}
