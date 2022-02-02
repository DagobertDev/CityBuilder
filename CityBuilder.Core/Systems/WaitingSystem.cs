using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class WaitingSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(float state, in Entity entity, ref Waiting waiting)
	{
		var value = waiting.RemainingDuration;
		value -= state;

		if (value > 0)
		{
			entity.Set<Waiting>(value);
		}
		else
		{
			entity.Remove<Waiting>();
			entity.Get<BehaviorState>().Next(out var next);
			entity.Set(next);
		}
	}
}
