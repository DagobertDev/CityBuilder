using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class WorkingDecisionSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, in Employee employee)
	{
		if (employee.Workplace.Has<CanNotWorkReason>() || employee.Workplace.Has<ResourceCollector>())
		{
			return;
		}

		entity.Set(Behavior.Working);
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;
}
