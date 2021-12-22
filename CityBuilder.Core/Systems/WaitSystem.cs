using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class WaitSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(float state, in Entity entity, ref Waiting waiting)
	{
		waiting = new Waiting(waiting.RemainingDuration - state);

		if (waiting.RemainingDuration <= 0)
		{
			entity.Remove<Waiting>();
			entity.Set<Idling>();
		}
	}
}